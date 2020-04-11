using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Destiny2;
using Destiny2.Definitions;
using Destiny2.Entities.Items;
using Destiny2.Responses;
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
        private readonly IDestiny2 _destiny;
        private readonly IManifest _manifest;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOptions<BungieSettings> _bungie;
        private readonly ILogger _logger;

        private static readonly ISet<uint> _defaultOrnaments = new HashSet<uint>
        {
            2931483505,
            702981643,
            1959648454,
        };

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

        public MaxPowerService(IDestiny2 destiny, IManifest manifest, IHttpContextAccessor contextAccessor,
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
            _logger.LogInformation($"Getting items for the {type} account {accountId} character {characterId}");
            var info = await GetProfile(type, accountId);
            if(info == null)
            {
                return null;
            }

            if(!info.Characters.Data.TryGetValue(characterId, out var character))
            {
                _logger.LogWarning($"Could not find character {characterId}");
                return null;
            }

            var items = await LoadItems(info);
            return await ComputeMaxPower(character.ClassHash, items);
        }

        public Task<IDictionary<long, IDictionary<ItemSlot.SlotHashes, Item>>> ComputeMaxPowerAsync(BungieMembershipType type, long accountId)
        {
            _logger.LogInformation($"Getting items for the {type} account {accountId}");
            return null;
        }

        public decimal ComputePower(IEnumerable<Item> items)
        {
            return MaxPower.ComputePower(items);
        }

        private Task<IEnumerable<Item>> LoadItems(DestinyProfileResponse info)
        {
            var itemComponents = info.CharacterEquipment.Data.Values // Equipped items on all characters
                .Concat(info.CharacterInventories.Data.Values) // Items in all character inventories
                .SelectMany(group => group.Items)
                .Concat(info.ProfileInventory.Data.Items); // Items in the Vault

            var itemInstances = info.ItemComponents.Instances.Data;

            return LoadItems(itemComponents, itemInstances);
        }

        private async Task<IEnumerable<Item>> LoadItems(IEnumerable<DestinyItemComponent> itemComponents,
            IDictionary<long, DestinyItemInstanceComponent> itemInstances)
        {
            var items = new List<Item>();
            foreach(var itemComponent in itemComponents)
            {
                var itemDef = await _manifest.LoadInventoryItem(itemComponent.ItemHash);
                var bucket = await _manifest.LoadBucket(itemDef.Inventory.BucketTypeHash);
                if(!ShouldInclude(bucket))
                {
                    continue;
                }

                string iconUrl = null;
                if(itemComponent.OverrideStyleItemHash != null && !_defaultOrnaments.Contains(itemComponent.OverrideStyleItemHash.Value))
                {
                    var overrideIcon = await _manifest.LoadInventoryItem(itemComponent.OverrideStyleItemHash.Value);
                    iconUrl = overrideIcon.DisplayProperties.Icon;
                }

                itemInstances.TryGetValue(itemComponent.ItemInstanceId, out DestinyItemInstanceComponent instance);
                items.Add(new Item(_bungie.Value.BaseUrl, itemComponent, itemDef, bucket, instance, iconUrl));
            }

            return items;
        }

        private static bool ShouldInclude(DestinyInventoryBucketDefinition bucket)
        {
            return _includedBuckets.Contains((ItemSlot.SlotHashes)bucket.Hash);
        }

        private async Task<DestinyProfileResponse> GetProfile(BungieMembershipType type, long accountId)
        {
            var accessToken = await _contextAccessor.HttpContext.GetTokenAsync("access_token");

            return await _destiny.GetProfile(accessToken, type, accountId,
                DestinyComponentType.ProfileInventories, DestinyComponentType.Characters,
                DestinyComponentType.CharacterInventories, DestinyComponentType.CharacterEquipment,
                DestinyComponentType.ItemInstances);
        }

        private async Task<IDictionary<ItemSlot.SlotHashes, Item>> ComputeMaxPower(uint classHash, IEnumerable<Item> items)
        {
            var classDef = await _manifest.LoadClass(classHash);

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
    }
}