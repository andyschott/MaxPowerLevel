using System.Threading.Tasks;
using Destiny2;
using Destiny2.Definitions;

namespace MaxPowerLevel.Services
{
    public class ManifestService : IManifestService
    {
        private readonly ManifestDb _db;
        
        public ManifestService(ManifestSettings manifestSettings)
        {
            _db = new ManifestDb(manifestSettings.DbPath.FullName);
        }
        
        public Task<DestinyClassDefinition> LoadClassAsync(uint hash)
        {
            return _db.LoadClass(hash);
        }
        
        public Task<DestinyInventoryItemDefinition> LoadInventoryItemAsync(uint hash)
        {
            return _db.LoadInventoryItem(hash);
        }

        public Task<DestinyInventoryBucketDefinition> LoadBucket(uint hash)
        {
            return _db.LoadBucket(hash);
        }
  }
}