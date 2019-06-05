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

        private static readonly ISet<ItemSlot.SlotHashes> _includedBuckets =
            new HashSet<ItemSlot.SlotHashes>
            {
                ItemSlot.SlotHashes.Kinetic,
                ItemSlot.SlotHashes.Energy,
                ItemSlot.SlotHashes.Power,
                ItemSlot.SlotHashes.Helmet,
                ItemSlot.SlotHashes.Gauntlets,
                ItemSlot.SlotHashes.ChestArmor,
                ItemSlot.SlotHashes.LegArmor,
                ItemSlot.SlotHashes.ClassArmor,
            };

        public MaxPowerService(IDestinyService destiny, IManifestService manifest)
        {
            _destiny = destiny;
            _manifest = manifest;
        }

        public async Task<IEnumerable<Item>> ComputeMaxPowerAsync(BungieMembershipType type, long accountId, long characterId)
        {
            // seems to be ignoring items in the vault
            // info.ProfileInventory.Data.Items has 246 items (should be in the 150's)
            var info = await _destiny.GetProfileAsync(type, accountId, DestinyComponentType.ProfileInventories,
                DestinyComponentType.Characters, DestinyComponentType.CharacterInventories,
                DestinyComponentType.CharacterEquipment, DestinyComponentType.ItemInstances);

            var classDef = await _manifest.LoadClassAsync(info.Characters.Data[characterId].ClassHash);

            var itemComponents = info.CharacterEquipment.Data.Values // Equipped items on all characters
                .Concat(info.CharacterInventories.Data.Values) // Items in all character inventories
                .SelectMany(group => group.Items)
                .Concat(info.ProfileInventory.Data.Items); // Items in the Vault

            var itemInstances = info.ItemComponents.Instances.Data;

            var items = await LoadItems(itemComponents, itemInstances);

            var gearSlots = items.Where(item => item.ClassType == DestinyClass.Unknown || item.ClassType == classDef.ClassType)
                .OrderByDescending(item => item.PowerLevel)
                .ToLookup(item => (ItemSlot.SlotHashes)item.Slot.Hash);

            var maxWeapons = MaxPower.FindMax(gearSlots[ItemSlot.SlotHashes.Kinetic],
                gearSlots[ItemSlot.SlotHashes.Energy],
                gearSlots[ItemSlot.SlotHashes.Power]);
            var maxArmor = MaxPower.FindMax(gearSlots[ItemSlot.SlotHashes.Helmet],
                gearSlots[ItemSlot.SlotHashes.Gauntlets],
                gearSlots[ItemSlot.SlotHashes.ChestArmor],
                gearSlots[ItemSlot.SlotHashes.LegArmor],
                gearSlots[ItemSlot.SlotHashes.ClassArmor]);
            
            return maxWeapons.Concat(maxArmor);
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
                var bucket = await _manifest.LoadBucket(itemDef.Inventory.BucketTypeHash);
                if(!ShouldInclude(bucket))
                {
                    continue;
                }

                itemInstances.TryGetValue(itemComponent.ItemInstanceId, out DestinyItemInstanceComponent instance);
                items.Add(new Item(itemComponent, itemDef, bucket, instance));
            }

            return items;
        }

        private static bool ShouldInclude(DestinyInventoryBucketDefinition bucket)
        {
            return _includedBuckets.Contains((ItemSlot.SlotHashes)bucket.Hash);
        }
    }
}