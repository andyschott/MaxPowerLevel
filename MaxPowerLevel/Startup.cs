using Destiny2;
using MaxPowerLevel.Helpers;
using MaxPowerLevel.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<BungieSettings>(Configuration.GetSection("Bungie"));
            var bungie = Configuration.GetSection("Bungie").Get<BungieSettings>();

            services.AddHttpContextAccessor();

            services.AddScoped<IMaxPowerService, MaxPowerService>();
            services.AddSingleton<IRecommendations, Season8Recommendations>();

            var config = new Destiny2Config(Configuration["AppName"], Configuration["AppVersion"],
                Configuration["AppId"], Configuration["Url"], Configuration["Email"])
            {
                BaseUrl = bungie.BaseUrl,
                ApiKey = bungie.ApiKey,
            };
            services.AddDestiny2(config);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddBungieAuthentication(new AuthenticationConfiguration
            {
                LoginCookieName = bungie.LoginCookieName,
                ClientId = bungie.ClientId,
                ClientSecret = bungie.ClientSecret,
                AuthorizationEndpoint = bungie.AuthorizationEndpoint,
                TokenEndpoint = bungie.TokenEndpoint,
                CallbackPath = "/signin-bungie/"
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // https://stackoverflow.com/a/43878365/3857
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                RequireHeaderSymmetry = false,
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

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
    }
}
