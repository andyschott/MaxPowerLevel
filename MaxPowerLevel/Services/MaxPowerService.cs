using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Destiny2;
using Destiny2.Definitions;
using Destiny2.Entities.Items;
using Destiny2.Responses;
using MaxPowerLevel.Helpers;
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

            var maxWeapons = MaxPower.FindMax(gearSlots[ItemSlot.KineticWeapon],
                gearSlots[ItemSlot.EnergyWeapon],
                gearSlots[ItemSlot.PowerWeapon]);
            var maxArmor = MaxPower.FindMax(gearSlots[ItemSlot.Helmet],
                gearSlots[ItemSlot.Gauntlet],
                gearSlots[ItemSlot.ChestArmor],
                gearSlots[ItemSlot.LegArmor],
                gearSlots[ItemSlot.ClassArmor]);
            
            return maxWeapons.Concat(maxArmor)
                .ToDictionary(item => item.Slot);
        }

        public int ComputePower(IEnumerable<Item> items)
        {
            return MaxPower.ComputePower(items);
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
    }
}