using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Destiny2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MaxPowerLevel.Services
{
  public class DownloadManifestService : IHostedService
  {
    private readonly IServiceProvider _services;
    private readonly ManifestSettings _manifestSettings;
    private readonly ILogger _logger;
    private const int ManifestCheckTimeout = 5 * 60 * 1000; // 5 minutes

    public DownloadManifestService(IServiceProvider services, ManifestSettings manifestSettings,
        ILogger<DownloadManifestService> logger)
    {
        _services = services;
        _manifestSettings = manifestSettings;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while(!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _logger?.LogInformation("Checking for an updated manifest");
                await CheckManifest(cancellationToken);
                _logger?.LogInformation($"Finshed checking for the manifest. Waiting {ManifestCheckTimeout} ms to try again.");
                await Task.Delay(ManifestCheckTimeout, cancellationToken);
            }
            catch(TaskCanceledException)
            {
                _logger?.LogInformation("Canceling checking for an updated manifest.");
            }
        }

        _logger?.LogInformation("Exiting the method to check for an updated manifest.");
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