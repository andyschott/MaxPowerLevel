using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Destiny2;
using Destiny2.Definitions;
using Destiny2.Entities.Items;
using MaxPowerLevel.Helpers;
using MaxPowerLevel.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MaxPowerLevel.Services
{
    public class MaxPowerService : IMaxPowerService
    {
        private readonly IDestiny _destiny;
        private readonly IManifestService _manifest;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOptions<BungieSettings> _bungie;
        private readonly ILogger _logger;

        private static readonly ISet<ItemSlot.SlotHashes> _includedBuckets =
            new HashSet<ItemSlot.SlotHashes>
            {
                ItemSlot.SlotHashes.Kinetic,
                ItemSlot.SlotHashes.Energy,
                ItemSlot.SlotHashes.Power,
                ItemSlot.SlotHashes.Helmet,
                ItemSlot.SlotHashes.Gauntlet,
                ItemSlot.SlotHashes.ChestArmor,
                ItemSlot.SlotHashes.LegArmor,
                ItemSlot.SlotHashes.ClassArmor,
            };

        public MaxPowerService(IDestiny destiny, IManifestService manifest, IHttpContextAccessor contextAccessor,
            IOptions<BungieSettings> bungie, ILogger<MaxPowerService> logger)
        {
            _destiny = destiny;
            _manifest = manifest;
            _contextAccessor = contextAccessor;
            _bungie = bungie;
            _logger = logger;
        }

        public async Task<IDictionary<ItemSlot.SlotHashes, Item>> ComputeMaxPowerAsync(BungieMembershipType type, long accountId,
            long characterId)
        {
            var accessToken = await _contextAccessor.HttpContext.GetTokenAsync("access_token");

            _logger.LogInformation($"Getting items for the {type} account {accountId} character {characterId}");
            var info = await _destiny.GetProfile(accessToken, type, accountId,
                DestinyComponentType.ProfileInventories, DestinyComponentType.Characters,
                DestinyComponentType.CharacterInventories, DestinyComponentType.CharacterEquipment,
                DestinyComponentType.ItemInstances);
            if(info == null)
            {
                return null;
            }

            if(!info.Characters.Data.TryGetValue(characterId, out var character))
            {
                _logger.LogWarning($"Could not find character {characterId}");
                return null;
            }

            var classDef = await _manifest.LoadClassAsync(character.ClassHash);

            var itemComponents = info.CharacterEquipment.Data.Values // Equipped items on all characters
                .Concat(info.CharacterInventories.Data.Values) // Items in all character inventories
                .SelectMany(group => group.Items)
                .Concat(info.ProfileInventory.Data.Items); // Items in the Vault

            var itemInstances = info.ItemComponents.Instances.Data;

            var items = await LoadItems(itemComponents, itemInstances);

            var gearSlots = items.Where(item => item.ClassType == DestinyClass.Unknown || item.ClassType == classDef.ClassType)
                .OrderByDescending(item => item.PowerLevel)
                .ToLookup(item => item.Slot.Hash);

            var maxWeapons = MaxPower.FindMax(gearSlots[ItemSlot.SlotHashes.Kinetic],
                gearSlots[ItemSlot.SlotHashes.Energy],
                gearSlots[ItemSlot.SlotHashes.Power]);
            var maxArmor = MaxPower.FindMax(gearSlots[ItemSlot.SlotHashes.Helmet],
                gearSlots[ItemSlot.SlotHashes.Gauntlet],
                gearSlots[ItemSlot.SlotHashes.ChestArmor],
                gearSlots[ItemSlot.SlotHashes.LegArmor],
                gearSlots[ItemSlot.SlotHashes.ClassArmor]);
            
            var maxItems = maxWeapons.Concat(maxArmor);

            _logger.LogDebug("Max power level items:");
            foreach(var item in maxItems)
            {
                _logger.LogDebug(item.ToString());
            }

            return maxItems.ToDictionary(item => (ItemSlot.SlotHashes)item.Slot.Hash);
        }

        public int ComputePower(IEnumerable<Item> items)
        {
            return MaxPower.ComputePower(items);
        }

        private async Task<IEnumerable<Item>> LoadItems(IEnumerable<DestinyItemComponent> itemComponents,
            IDictionary<long, DestinyItemInstanceComponent> itemInstances)
        {
            var items = new List<Item>();
            foreach(var itemComponent in itemComponents)
            {
                var itemDef = await _manifest.LoadInventoryItemAsync(itemComponent.ItemHash);
                var bucket = await _manifest.LoadBucket(itemDef.Inventory.BucketTypeHash);
                if(!ShouldInclude(bucket))
                {
                    continue;
                }

                itemInstances.TryGetValue(itemComponent.ItemInstanceId, out DestinyItemInstanceComponent instance);
                items.Add(new Item(_bungie.Value.BaseUrl, itemComponent, itemDef, bucket, instance));
            }

            return items;
        }

        private static bool ShouldInclude(DestinyInventoryBucketDefinition bucket)
        {
            return _includedBuckets.Contains((ItemSlot.SlotHashes)bucket.Hash);
        }
    }
}