using System.Collections.Generic;

namespace MaxPowerLevel.Models
{
    public class CharacterViewModel
    {
        public IList<Item> Items { get; set; } = new List<Item>();
    }
}