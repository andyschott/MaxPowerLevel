using System.Threading.Tasks;
using Destiny2;
using Destiny2.Definitions;
using Microsoft.AspNetCore.Http;

namespace MaxPowerLevel.Services
{
    public class ManifestService : IManifestService
    {
        private readonly ManifestDb _db;
        
        public ManifestService(IHttpContextAccessor contextAccessor)
        {
            var dbPath = (string)contextAccessor.HttpContext.Items["ManifestDbPath"];
            _db = new ManifestDb(dbPath);
        }
        
        public Task<DestinyClassDefinition> LoadClassAsync(uint hash)
        {
            return _db.LoadClass(hash);
        }
        
        public Task<DestinyInventoryItemDefinition> LoadInventoryItemAsync(uint hash)
        {
            return _db.LoadInventoryItem(hash);
        }
  }
}