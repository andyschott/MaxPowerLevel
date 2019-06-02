using Destiny2;
using Destiny2.Definitions;
using Destiny2.Entities.Items;

namespace MaxPowerLevel.Models
{
    public class Item
    {
        public Item(DestinyItemComponent itemComponent, DestinyInventoryItemDefinition itemDef, DestinyItemInstanceComponent instance = null)
        {
            Name = itemDef.DisplayProperties.Name;
            Slot = (ItemSlot)itemComponent.BucketHash;
            PowerLevel = instance?.PrimaryStat?.Value ?? 0;
            Tier = itemDef.Inventory.TierType;
        }

        public string Name { get; }
        public ItemSlot Slot { get; }
        public int PowerLevel { get; }
        public TierType Tier { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}