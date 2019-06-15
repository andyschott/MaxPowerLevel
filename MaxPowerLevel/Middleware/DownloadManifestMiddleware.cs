using System;
using System.IO;
using System.Threading.Tasks;
using Destiny2;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace MaxPowerLevel.Middleware
{
    public class DownloadManifestMiddleware
    {
        private readonly RequestDelegate _next;

        private static readonly FileInfo _manifestVersionPath;
        private static readonly FileInfo _manifestDbPath;

        static DownloadManifestMiddleware()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dir = Path.Combine(localAppData, "MaxPowerLevel");
            if(!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            _manifestVersionPath = new FileInfo(Path.Combine(dir, "ManifestVersion.txt"));
            _manifestDbPath = new FileInfo(Path.Combine(dir, "Manifest.db"));
        }

        public DownloadManifestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var downloader = (IManifestDownloader)context.RequestServices.GetService(typeof(IManifestDownloader));

            var currentVersion = await GetCurrentManifestVersion();
            var updatedVersion = await downloader.DownloadManifest(_manifestDbPath.FullName, currentVersion);
            if(!string.IsNullOrEmpty(updatedVersion))
            {
                Task t = UpdateCurrentManifestVersion(updatedVersion);
            }

            context.Items.Add("ManifestDbPath", _manifestDbPath.FullName);

            await _next(context);
        }

        private static async Task<string> GetCurrentManifestVersion()
        {
            if (!_manifestVersionPath.Exists)
            {
                return string.Empty;
            }

            return await File.ReadAllTextAsync(_manifestVersionPath.FullName);
        }

        private static Task UpdateCurrentManifestVersion(string updatedVersion)
        {
            return File.WriteAllTextAsync(_manifestVersionPath.FullName, updatedVersion);
        }
    }

    public static class DownloadManifestMiddlewareExtensions
    {
        public static IApplicationBuilder UseDownloadManifest(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DownloadManifestMiddleware>();
        }
    }
}