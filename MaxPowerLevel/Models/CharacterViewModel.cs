using System.Collections.Generic;
using System.Linq;
using Destiny2;

namespace MaxPowerLevel.Models
{
    public class CharacterViewModel
    {
        public BungieMembershipType Type { get; set; }
        public long AccountId { get; set; }
        public IEnumerable<Item> Items { get; set; } = Enumerable.Empty<Item>();
        public int MaxPower { get; set; } = 0;
        public int BonusPower { get; set; } = 0;
        public string EmblemPath { get; set; }
        public string EmblemBackgroundPath {get; set; }

        public IEnumerable<Item> Weapons => Items.Where(item => item.IsWeapon)
                                                 .OrderBy(item => item.Slot.Order);
        
        public IEnumerable<Item> Armor => Items.Where(item => item.IsArmor)
                                               .OrderBy(item => item.Slot.Order);

        public IEnumerable<Item> LowestItems { get; set; } = Enumerable.Empty<Item>();
        public IEnumerable<string> Recommendations { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<Engram> Engrams { get; set; }
    }
}