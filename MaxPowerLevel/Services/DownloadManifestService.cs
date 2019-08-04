using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Destiny2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace MaxPowerLevel.Services
{
  public class DownloadManifestService : IHostedService
  {
    private readonly IServiceProvider _services;
    private readonly ManifestSettings _manifestSettings;

    private const int ManifestCheckTimeout = 5 * 60 * 1000; // 5 minutes

    public DownloadManifestService(IServiceProvider services, ManifestSettings manifestSettings)
    {
        _services = services;
        _manifestSettings = manifestSettings;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while(!cancellationToken.IsCancellationRequested)
        {
            await CheckManifest(cancellationToken);
            await Task.Delay(ManifestCheckTimeout, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task CheckManifest(CancellationToken cancellationToken)
    {
        using(var scope = _services.CreateScope())
        {
            var downloader = (IManifestDownloader)scope.ServiceProvider.GetRequiredService(typeof(IManifestDownloader));

            var currentVersion = await GetCurrentManifestVersion(cancellationToken);
            var updatedVersion = await downloader.DownloadManifest(_manifestSettings.DbPath.FullName, currentVersion);

            if(!string.IsNullOrEmpty(updatedVersion))
            {
                Task t = UpdateCurrentManifestVersion(updatedVersion, cancellationToken);
            }
        }
    }

    private async Task<string> GetCurrentManifestVersion(CancellationToken cancellationToken)
    {
        if (!_manifestSettings.VersionPath.Exists)
        {
            return string.Empty;
        }

        return await File.ReadAllTextAsync(_manifestSettings.VersionPath.FullName,
            cancellationToken);
    }

    private Task UpdateCurrentManifestVersion(string updatedVersion, CancellationToken cancellationToken)
    {
        return File.WriteAllTextAsync(_manifestSettings.VersionPath.FullName, updatedVersion,
            cancellationToken);
    }
  }
}