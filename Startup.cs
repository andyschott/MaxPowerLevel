using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using MaxPowerLevel.Helpers;
using MaxPowerLevel.Middleware;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

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


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "Bungie";
            })
            .AddCookie(options => {
                options.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = HandleRefreshToken
                };
            })
            .AddOAuth("Bungie", options => {
                options.ClientId = Configuration["Bungie:ClientId"];
                options.ClientSecret = Configuration["Bungie:ClientSecret"];
                options.CallbackPath = new PathString("/signin-bungie/");

                options.AuthorizationEndpoint = "https://www.bungie.net/en/oauth/authorize";
                options.TokenEndpoint = "https://www.bungie.net/platform/app/oauth/token/";

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
            app.UseDownloadManifest(Configuration["Bungie:ApiKey"]);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // https://stackoverflow.com/q/52175302/3857
        private async Task HandleRefreshToken(CookieValidatePrincipalContext context)
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

            // Token is expired. Attempt to renew it.
            var request = new RefreshTokenRequest
            {
                Address = "https://www.bungie.net/platform/app/oauth/token/",
                ClientId = Configuration["Bungie:ClientId"],
                ClientSecret = Configuration["Bungie:ClientSecret"],
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
