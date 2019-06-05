using System.Collections.Generic;
using System.Linq;
using Destiny2;

namespace MaxPowerLevel.Models
{
    public class CharacterViewModel
    {
        public IDictionary<ItemSlot.SlotHashes, Item> Items { get; set; } = new Dictionary<ItemSlot.SlotHashes, Item>();
        public int MaxPower { get; set; } = 0; 

        public Item KineticWeapon => Items[ItemSlot.SlotHashes.Kinetic];
        public Item EnergyWeapon => Items[ItemSlot.SlotHashes.Energy];
        public Item PowerWeapon => Items[ItemSlot.SlotHashes.Power];
        public Item Helmet => Items[ItemSlot.SlotHashes.Helmet];
        public Item Gauntlet => Items[ItemSlot.SlotHashes.Gauntlet];
        public Item ChestArmor => Items[ItemSlot.SlotHashes.ChestArmor];
        public Item LegArmor => Items[ItemSlot.SlotHashes.LegArmor];
        public Item ClassArmor => Items[ItemSlot.SlotHashes.ClassArmor];
    }
}