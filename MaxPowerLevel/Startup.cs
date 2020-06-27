using System;
using Destiny2;
using MaxPowerLevel.Helpers;
using MaxPowerLevel.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VendorEngrams;

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
            services.Configure<BungieSettings>(Configuration.GetSection("Bungie"));
            var bungie = Configuration.GetSection("Bungie").Get<BungieSettings>();

            services.AddHttpContextAccessor();

            services.AddScoped<IMaxPowerService, MaxPowerService>();
            services.AddScoped<SeasonPass>();
            services.AddScoped<ChargedWithLight>();
            AddRecommendations(services);

            var config = new Destiny2Config(Configuration["AppName"], Configuration["AppVersion"],
                Configuration["AppId"], Configuration["Url"], Configuration["Email"])
            {
                BaseUrl = bungie.BaseUrl,
                ApiKey = bungie.ApiKey,
            };
            services.AddDestiny2(config);
            services.AddVendorEngramsClient("destiny2utils");

            services.AddRazorPages();

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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
            var options = new ForwardedHeadersOptions
            {
                RequireHeaderSymmetry = false,
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
            app.UseForwardedHeaders(options);

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();

                endpoints.MapControllers();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // private static readonly DateTime Season11StartDate =
        //     new DateTime(2020, 6, 9, 18, 0, 0, DateTimeKind.Utc);

        private void AddRecommendations(IServiceCollection services)
        {
            services.AddScoped<IRecommendations>(sp =>
            {
                var manifest = sp.GetRequiredService<IManifest>();
                var vendorEngrams = sp.GetRequiredService<IVendorEngramsClient>();
                var seasonPass = sp.GetRequiredService<SeasonPass>();

                // if(DateTime.UtcNow >= Season11StartDate)
                // {
                //     return new Season11Recommendations(manifest, vendorEngrams, seasonPass);
                // }

                return new Season11Recommendations(manifest, vendorEngrams, seasonPass);
            });
        }
    }
}
