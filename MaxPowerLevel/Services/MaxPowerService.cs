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

        public async Task<IDictionary<long, IDictionary<ItemSlot.SlotHashes, Item>>> ComputeMaxPower(IDictionary<long, DestinyCharacterComponent> characters,
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

        public IEnumerable<Item> FindLowestItems(IEnumerable<Item> items)
        {
                var minPower = items.Min(item => item.PowerLevel);
                var lowestItems = items.OrderBy(item => item.PowerLevel)
                                       .TakeWhile(item => item.PowerLevel == minPower);
                if(lowestItems.Count() == items.Count())
                {
                    // All items are max power.
                    return Enumerable.Empty<Item>();
                }

                return lowestItems;
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
                string watermark = null;
                if(itemComponent.OverrideStyleItemHash != null && !_defaultOrnaments.Contains(itemComponent.OverrideStyleItemHash.Value))
                {
                    var overrideIcon = await _manifest.LoadInventoryItem(itemComponent.OverrideStyleItemHash.Value);
                    iconUrl = overrideIcon.DisplayProperties.Icon;

                    watermark = GetWatermarkIcon(overrideIcon);
                }
                else
                {
                    watermark = GetWatermarkIcon(itemDef);
                }

                itemInstances.TryGetValue(itemComponent.ItemInstanceId, out DestinyItemInstanceComponent instance);
                items.Add(new Item(_bungie.Value.BaseUrl, itemComponent, itemDef, bucket, instance, iconUrl, watermark));
            }

            return items;
        }

        private static string GetWatermarkIcon(DestinyInventoryItemDefinition itemDef)
        {
            if(itemDef.Quality == null || itemDef.Quality.CurrentVersion < 0)
            {
                return string.Empty;
            }

            return itemDef.Quality.DisplayVersionWatermarkIcons.Skip(itemDef.Quality.CurrentVersion)
                .FirstOrDefault();
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

            // As of Season 11, the game seems to ignore the "only one exotic"
            // rule when computing max power. So at least for now, ignore that
            // here as well.
            var maxItems = gearSlots.Select(items => items.First())
                .OrderBy(item => item.Slot.Order);

            _logger.LogDebug("Max power level items:");
            foreach(var item in maxItems)
            {
                _logger.LogDebug(item.ToString());
            }

            return maxItems.ToDictionary(item => item.Slot.Hash);
        }
    }
}