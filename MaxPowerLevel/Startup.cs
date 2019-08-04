using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Destiny2.Extensions;
using IdentityModel.Client;
using MaxPowerLevel.Helpers;
using MaxPowerLevel.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MaxPowerLevel
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSingleton<ManifestSettings>();

            services.Configure<BungieSettings>(Configuration.GetSection("Bungie"));
            var bungie = Configuration.GetSection("Bungie").Get<BungieSettings>();

            services.AddHttpContextAccessor();

            services.AddScoped<IManifestService, ManifestService>();
            services.AddScoped<IMaxPowerService, MaxPowerService>();

            services.AddDestiny(bungie.BaseUrl, bungie.ApiKey);
            services.AddManifestDownloader();

            services.AddHostedService<DownloadManifestService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "Bungie";
            })
            .AddCookie(options => {
                options.Cookie.Name = bungie.LoginCookieName;
                options.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = context =>
                    {
                        return HandleRefreshToken(context, bungie.TokenEndpoint);
                    }
                };
            })
            .AddOAuth("Bungie", options => {
                options.ClientId = bungie.ClientId;
                options.ClientSecret = bungie.ClientSecret;
                options.CallbackPath = new PathString("/signin-bungie/");

                options.AuthorizationEndpoint = bungie.AuthorizationEndpoint;
                options.TokenEndpoint = bungie.TokenEndpoint;

                options.SaveTokens = true;

                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "membership_id");

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = context =>
                    {
                        context.RunClaimActions(context.TokenResponse.Response);
                        return Task.CompletedTask;
                    },
                    OnRemoteFailure = context =>
                    {
                        return Task.CompletedTask;
                    }
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // https://stackoverflow.com/q/52175302/3857
        private async Task HandleRefreshToken(CookieValidatePrincipalContext context, string tokenEndpoint)
        {
            if(!context.Principal.Identity.IsAuthenticated)
            {
                return;
            }

            var tokens = context.Properties.GetTokens();
            var refreshToken = tokens.FirstOrDefault(t => t.Name == "refresh_token");
            var accessToken = tokens.FirstOrDefault(t => t.Name == "access_token");
            var exp = tokens.FirstOrDefault(t => t.Name == "expires_at");
            var expires = DateTime.Parse(exp.Value);
            if(expires >= DateTime.Now)
            {
                return;
            }

            var bungie = Configuration.GetSection("Bungie").Get<BungieSettings>();

            // Token is expired. Attempt to renew it.
            var request = new RefreshTokenRequest
            {
                Address = tokenEndpoint,
                ClientId = bungie.ClientId,
                ClientSecret = bungie.ClientSecret,
                RefreshToken = refreshToken.Value
            };
            TokenResponse tokenResponse;
            using(var client = new HttpClient())
            {
                tokenResponse = await client.RequestRefreshTokenAsync(request);
            }

            if(tokenResponse.IsError)
            {
                context.RejectPrincipal();
                return;
            }

            refreshToken.Value = tokenResponse.RefreshToken;
            accessToken.Value = tokenResponse.AccessToken;

            var newExpires = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn);
            exp.Value = newExpires.ToString("o", CultureInfo.InvariantCulture);

            context.Properties.StoreTokens(tokens);
            context.ShouldRenew = true;
        }
    }
}
