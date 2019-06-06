using System.Collections.Generic;
using System.Linq;
using Destiny2;

namespace MaxPowerLevel.Models
{
    public class CharacterViewModel
    {
        public IEnumerable<Item> Items { get; set; } = new List<Item>();
        public int MaxPower { get; set; } = 0;

        public IEnumerable<Item> Weapons => Items.Where(item => item.IsWeapon)
                                                 .OrderBy(item => item.Slot.Order);
        
        public IEnumerable<Item> Armor => Items.Where(item => item.IsArmor)
                                               .OrderBy(item => item.Slot.Order);

        public IEnumerable<Item> LowestItems
        {
            get
            {
                var minPower = Items.Min(item => item.PowerLevel);
                return Items.OrderBy(item => item.PowerLevel)
                            .TakeWhile(item => item.PowerLevel == minPower);
            }
        }
    }
}