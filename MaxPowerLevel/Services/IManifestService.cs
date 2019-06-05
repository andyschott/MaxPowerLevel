using System.Threading.Tasks;
using Destiny2.Definitions;

namespace MaxPowerLevel.Services
{
    public interface IManifestService
    {
        Task<DestinyClassDefinition> LoadClassAsync(uint hash);
        Task<DestinyInventoryItemDefinition> LoadInventoryItemAsync(uint hash);
        Task<DestinyInventoryBucketDefinition> LoadBucket(uint hash);
    }
}