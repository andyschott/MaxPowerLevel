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
        private ManifestDownloader _downloader = null;
        private string _apiKey;

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

        public DownloadManifestMiddleware(RequestDelegate next, string apiKey)
        {
            _next = next;
            _apiKey = apiKey;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (null == _downloader)
            {
                var currentVersion = await GetCurrentManifestVersion();
                _downloader = new ManifestDownloader(_apiKey, _manifestDbPath.FullName, currentVersion);
                _apiKey = null;
            }

            var updatedVersion = await _downloader.DownloadManifest();
            if(!string.IsNullOrEmpty(updatedVersion))
            {
                Task t = UpdateCurrentManifestVersion(updatedVersion);
            }

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
        public static IApplicationBuilder UseDownloadManifest(this IApplicationBuilder builder, string apiKey)
        {
            return builder.UseMiddleware<DownloadManifestMiddleware>(apiKey);
        }
    }
}