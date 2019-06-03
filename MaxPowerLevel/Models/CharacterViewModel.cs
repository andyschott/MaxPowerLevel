using System.Collections.Generic;
using Destiny2;

namespace MaxPowerLevel.Models
{
    public class CharacterViewModel
    {
        public IDictionary<ItemSlot, Item> Items { get; set; } = new Dictionary<ItemSlot, Item>();

        public Item KineticWeapon
        {
            get { return Items[ItemSlot.KineticWeapon]; }
        }

        public Item EnergyWeapon
        {
            get { return Items[ItemSlot.EnergyWeapon]; }
        }

        public Item PowerWeapon
        {
            get { return Items[ItemSlot.PowerWeapon]; }
        }

        public Item Helmet
        {
            get { return Items[ItemSlot.Helmet]; }
        }

        public Item Gauntlet
        {
            get { return Items[ItemSlot.Gauntlet]; }
        }

        public Item ChestArmor
        {
            get { return Items[ItemSlot.ChestArmor]; }
        }

        public Item LegArmor
        {
            get { return Items[ItemSlot.LegArmor]; }
        }

        public Item ClassItem
        {
            get { return Items[ItemSlot.ClassArmor]; }
        }
    }
}