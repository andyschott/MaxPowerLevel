using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Destiny2;
using Destiny2.Entities.Items;
using MaxPowerLevel.Helpers;
using MaxPowerLevel.Models;
using Microsoft.Extensions.Options;

namespace MaxPowerLevel.Services
{
    public class ItemService
    {
        private readonly IManifest _manifest;
        private readonly IOptions<BungieSettings> _bungie;

        public ItemService(IManifest manifest, IOptions<BungieSettings> bungie)
        {
            _manifest = manifest;
            _bungie = bungie;
        }

        public async Task<IEnumerable<Item>> GetEngrams(IEnumerable<DestinyItemComponent> inventory,
            IDictionary<long, DestinyItemInstanceComponent> itemInstances)
        {
            var engramItemComponents = inventory.Where(itemComponent => itemComponent.BucketHash == (uint)ItemSlot.SlotHashes.Engrams);
            var bucket = await _manifest.LoadBucket((uint)ItemSlot.SlotHashes.Engrams);

            var tasks = engramItemComponents.Select(async itemComponent =>
            {
                var itemDef = await _manifest.LoadInventoryItem(itemComponent.ItemHash);
                itemInstances.TryGetValue(itemComponent.ItemInstanceId, out DestinyItemInstanceComponent instance);

                return new Item(_bungie.Value.BaseUrl, itemComponent, itemDef, bucket,
                    instance);
            });

            return await Task.WhenAll(tasks); 
        }
    }
}