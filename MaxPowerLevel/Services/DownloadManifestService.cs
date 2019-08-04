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
  public class DownloadManifestService : IHostedService, IDisposable
  {
    private readonly IServiceProvider _services;
    private readonly ManifestSettings _manifestSettings;
    private readonly ILogger _logger;

    private Timer _timer = null;

    private const int ManifestCheckTimeout = 5 * 60 * 1000; // 5 minutes

    public DownloadManifestService(IServiceProvider services, ManifestSettings manifestSettings,
        ILogger<DownloadManifestService> logger)
    {
        _services = services;
        _manifestSettings = manifestSettings;
        _logger = logger;
    }

    public void Dispose()
    {
        _timer.Dispose();
        _timer = null;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(async (state) => await CheckManifest(state), cancellationToken,
            TimeSpan.Zero, TimeSpan.FromMilliseconds(ManifestCheckTimeout));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    private async Task CheckManifest(object state)
    {
        _logger?.LogInformation("Checking for an updated manifest.");

        var cancellationToken = (CancellationToken)state;

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