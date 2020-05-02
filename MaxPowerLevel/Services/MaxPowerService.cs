using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Destiny2;
using Destiny2.Definitions;
using Destiny2.Entities;
using Destiny2.Entities.Items;
using MaxPowerLevel.Helpers;
using MaxPowerLevel.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MaxPowerLevel.Services
{
    public class MaxPowerService : IMaxPowerService
    {
        private readonly IDestiny2 _destiny;
        private readonly IManifest _manifest;
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

        public MaxPowerService(IDestiny2 destiny, IManifest manifest, IOptions<BungieSettings> bungie,
            ILogger<MaxPowerService> logger)
        {
            _destiny = destiny;
            _manifest = manifest;
            _bungie = bungie;
            _logger = logger;
        }

        public async Task<IDictionary<ItemSlot.SlotHashes, Item>> ComputeMaxPower(DestinyCharacterComponent character,
            IEnumerable<DestinyInventoryComponent> characterEquipment,
            IEnumerable<DestinyInventoryComponent> characterInventories,
            DestinyInventoryComponent vault,
            IDictionary<long, DestinyItemInstanceComponent> itemInstances)
        {
            var items = await LoadItems(characterEquipment, characterInventories,
                vault, itemInstances);
            return await ComputeMaxPower(character.ClassHash, items);
        }

        public async Task<IDictionary<long, IDictionary<ItemSlot.SlotHashes, Item>>> ComputeMaxPowerAsync(IDictionary<long, DestinyCharacterComponent> characters,
            IEnumerable<DestinyInventoryComponent> characterEquipment,
            IEnumerable<DestinyInventoryComponent> characterInventories,
            DestinyInventoryComponent vault,
            IDictionary<long, DestinyItemInstanceComponent> itemInstances)
        {
            var items = await LoadItems(characterEquipment, characterInventories,
                vault, itemInstances);

            var maxPowerTasks = characters.Select(async character =>
            {
                return (character.Key, maxPower: await ComputeMaxPower(character.Value.ClassHash, items));
            });

            await Task.WhenAll(maxPowerTasks);

            return maxPowerTasks.ToDictionary(item => item.Result.Key, item => item.Result.maxPower);
        }

        public decimal ComputePower(IEnumerable<Item> items)
        {
            return MaxPower.ComputePower(items);
        }

        private Task<IEnumerable<Item>> LoadItems(IEnumerable<DestinyInventoryComponent> characterEquipment,
            IEnumerable<DestinyInventoryComponent> characterInventories,
            DestinyInventoryComponent vault,
            IDictionary<long, DestinyItemInstanceComponent> itemInstances)
        {
           var itemComponents = characterEquipment // Equipped items on all characters
                .Concat(characterInventories) // Items in all character inventories
                .SelectMany(group => group.Items)
                .Concat(vault.Items); // Items in the Vault

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