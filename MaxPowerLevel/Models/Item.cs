using System.Collections.Generic;
using System.Linq;
using Destiny2;
using Destiny2.Definitions;
using Destiny2.Entities.Items;

namespace MaxPowerLevel.Models
{
    public class Item
    {
        public Item(string baseUrl, DestinyItemComponent itemComponent, DestinyInventoryItemDefinition itemDef,
            DestinyInventoryBucketDefinition bucket, DestinyItemInstanceComponent instance = null)
        {
            Name = itemDef.DisplayProperties.Name;
            PowerLevel = instance?.PrimaryStat?.Value ?? 0;
            Slot = new ItemSlot(bucket);
            Tier = itemDef.Inventory.TierType;
            ClassType = itemDef.ClassType;
            Icon = baseUrl + itemDef.DisplayProperties.Icon;
        }

        public Item(string name, ItemSlot.SlotHashes slot, int powerLevel,
            TierType tier = TierType.Superior, DestinyClass classType = DestinyClass.Unknown)
        {
            Name = name;
            Slot = new ItemSlot(slot.ToString(), slot);
            PowerLevel = powerLevel;
            Tier = tier;
            ClassType = classType;
        }

        public string Name { get; }
        public ItemSlot Slot { get; }
        public int PowerLevel { get; }
        public TierType Tier { get; }
        public DestinyClass ClassType { get; }
        public string Icon { get; }

        public bool IsWeapon => Slot.IsWeapon;
        public bool IsArmor => Slot.IsArmor;

        public override bool Equals(object obj)
        {
            if(!(obj is Item item))
            {
                return false;
            }

            return Name.Equals(item.Name) &&
                Slot == item.Slot &&
                PowerLevel == item.PowerLevel &&
                Tier == item.Tier &&
                ClassType == item.ClassType;
        }

        public override int GetHashCode()
        {
            return 23 ^ Name.GetHashCode() ^
                Slot.GetHashCode() ^
                PowerLevel.GetHashCode() ^
                Tier.GetHashCode() ^
                ClassType.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Name} ({PowerLevel})";
        }
    }
}