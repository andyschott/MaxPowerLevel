using System;
using System.IO;

namespace MaxPowerLevel.Services
{
    public class ManifestSettings
    {
        public FileInfo DbPath { get; }
        public FileInfo VersionPath { get; }

        public ManifestSettings()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dir = Path.Combine(localAppData, "MaxPowerLevel");
            if(!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            VersionPath = new FileInfo(Path.Combine(dir, "ManifestVersion.txt"));
            DbPath = new FileInfo(Path.Combine(dir, "Manifest.db"));
        }
    }
}