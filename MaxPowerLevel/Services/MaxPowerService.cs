using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Destiny2;
using Destiny2.Definitions;
using Destiny2.Entities.Items;
using Destiny2.Responses;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services
{
    public class MaxPowerService : IMaxPowerService
    {
        private readonly IDestinyService _destiny;
        private readonly IManifestService _manifest;

        private readonly TaskFactory _factory = new TaskFactory();

        private static readonly ISet<ItemSlot> _includedSlots =
            new HashSet<ItemSlot>
            {
                ItemSlot.KineticWeapon,
                ItemSlot.EnergyWeapon,
                ItemSlot.PowerWeapon,
                ItemSlot.Helmet,
                ItemSlot.Gauntlet,
                ItemSlot.ChestArmor,
                ItemSlot.LegArmor,
                ItemSlot.ClassArmor,
            };

        public MaxPowerService(IDestinyService destiny, IManifestService manifest)
        {
            _destiny = destiny;
            _manifest = manifest;
        }

        public async Task<IDictionary<ItemSlot, Item>> ComputeMaxPowerAsync(BungieMembershipType type, long accountId, long characterId)
        {
            var info = await _destiny.GetProfileAsync(type, accountId, DestinyComponentType.ProfileInventories,
                DestinyComponentType.Characters, DestinyComponentType.CharacterInventories,
                DestinyComponentType.CharacterEquipment, DestinyComponentType.ItemInstances);

            var classDef = await _manifest.LoadClassAsync(info.Characters.Data[characterId].ClassHash);

            var itemComponents = info.CharacterEquipment.Data.Values // Equipped items on all characters
                .Concat(info.CharacterInventories.Data.Values) // Items in all character inventories
                .SelectMany(group => group.Items)
                .Concat(info.ProfileInventory.Data.Items) // Items in the Vault
                .Where(item => _includedSlots.Contains((ItemSlot)item.BucketHash));

            var itemInstances = info.ItemComponents.Instances.Data;
            var items = await LoadItems(itemComponents, itemInstances);

            var gearSlots = items.Where(item => item.ClassType == DestinyClass.Unknown || item.ClassType == classDef.ClassType)
                .OrderByDescending(item => item.PowerLevel)
                .ToLookup(item => item.Slot);

            var maxWeapons = FindMax(gearSlots[ItemSlot.KineticWeapon],
                gearSlots[ItemSlot.EnergyWeapon],
                gearSlots[ItemSlot.PowerWeapon]);
            var maxArmor = FindMax(gearSlots[ItemSlot.Helmet],
                gearSlots[ItemSlot.Gauntlet],
                gearSlots[ItemSlot.ChestArmor],
                gearSlots[ItemSlot.LegArmor],
                gearSlots[ItemSlot.ClassArmor]);
            
            return maxWeapons.Concat(maxArmor)
                .ToDictionary(item => item.Slot);
        }

        private (DestinyItemComponent Item, DestinyItemInstanceComponent Instance) LoadItem(DestinyItemComponent itemComponent, IDictionary<long, DestinyItemInstanceComponent> itemInstances)
        {
            itemInstances.TryGetValue(itemComponent.ItemInstanceId, out DestinyItemInstanceComponent instance);
            return (itemComponent, instance);
        }

        private async Task<IEnumerable<Item>> LoadItems(IEnumerable<DestinyItemComponent> itemComponents,
            IDictionary<long, DestinyItemInstanceComponent> itemInstances)
        {
            var items = new List<Item>();
            foreach(var itemComponent in itemComponents)
            {
                var itemDef = await _manifest.LoadInventoryItemAsync(itemComponent.ItemHash);
                itemInstances.TryGetValue(itemComponent.ItemInstanceId, out DestinyItemInstanceComponent instance);
                items.Add(new Item(itemComponent, itemDef, instance));
            }

            return items;
        }

        private IEnumerable<Item> FindMax(params IEnumerable<Item>[] items)
        {
            if(!items.Any())
            {
                return Enumerable.Empty<Item>();
            }

            // The first item can just be added to the list.
            var maxItems = new List<Item>();
            maxItems.Add(items[0].First());
            int? exoticIndex = null;

            for(var index = 1; index < items.Length; ++index)
            {
                var maxItem = items[index].First();
                if(maxItem.Tier != TierType.Exotic)
                {
                    // This item isn't exotic. Add it to the list and move on.
                    maxItems.Add(maxItem);
                    continue;
                }

                // If a previous item is an exotic, determine which slot
                // has the higher non-exotic.
                if(exoticIndex == null)
                {
                    // No other exotics. Use this one and move on to the
                    // next slot.
                    maxItems.Add(maxItem);
                    exoticIndex = index;
                    continue;
                }
 
                var prevExotic = maxItems[exoticIndex.Value];
                if(prevExotic.PowerLevel == maxItem.PowerLevel)
                {
                    var prevLegendaryMaxItem = items[exoticIndex.Value]
                        .FirstOrDefault(item => item.Tier != TierType.Exotic);
                    if(null == prevLegendaryMaxItem)
                    {
                        // Only exotics in the previous slot. Use a non-exotic
                        // in this slot.
                        maxItem = items[index].First(item => item.Tier != TierType.Exotic);
                        maxItems.Add(maxItem);
                        continue;
                    }

                    var curLegendaryMaxItem = items[index]
                        .FirstOrDefault(item => item.Tier != TierType.Exotic);
                    if(null == curLegendaryMaxItem)
                    {
                        // Only exotics in the current slot. Use a non-exotic
                        // in the previous slot.
                        var prevMaxItem = items[exoticIndex.Value]
                            .First(item => item.Tier != TierType.Exotic);
                        maxItems[exoticIndex.Value] = prevMaxItem;

                        maxItems.Add(maxItem);
                        exoticIndex = index;
                        continue;
                    }

                    if(prevLegendaryMaxItem.PowerLevel > curLegendaryMaxItem.PowerLevel)
                    {
                        // Use the exotic in this slot instead of the previous one.
                        maxItems[exoticIndex.Value] = prevLegendaryMaxItem;
                        maxItems.Add(maxItem);
                        exoticIndex = index;
                        continue;
                    }
                    else
                    {
                        // Use the exotic in the previous slot.
                        maxItems.Add(curLegendaryMaxItem);
                        continue;
                    }
                }
                else if(prevExotic.PowerLevel > maxItem.PowerLevel)
                {
                    // Use the exotic in the previous slot.
                    var maxLegendary = items[index]
                        .FirstOrDefault(item => item.Tier != TierType.Exotic);
                    if(null == maxLegendary)
                    {
                        // No non-exotics in this slot. Use a legendary in the
                        // previous slot.
                        var prevMaxItem = items[exoticIndex.Value]
                            .First(item => item.Tier != TierType.Exotic);
                        maxItems[exoticIndex.Value] = prevMaxItem;

                        maxItems.Add(maxItem);
                        exoticIndex = index;
                        continue;
                    }
                    else
                    {
                        maxItems.Add(maxLegendary);
                        continue;
                    }
                }
                else // prevExotic.PowerLevel < maxItem.powerLevel
                {
                    // use the exotic in this slot.
                    var prevMaxLegendary = items[exoticIndex.Value]
                        .FirstOrDefault(item => item.Tier != TierType.Exotic);
                    if(null == prevMaxLegendary)
                    {
                        // No non-exitcs in the previous slot. Use a 
                        // legendary in this slot.
                        maxItem = items[index].First(item => item.Tier != TierType.Exotic);
                        maxItems.Add(maxItem);
                        continue;
                    }
                    else
                    {
                        maxItems[exoticIndex.Value] = prevMaxLegendary;

                        maxItems[index] = maxItem;
                        exoticIndex = index;
                        continue;
                    }
                }
            }

             return maxItems;
        }
    }
}