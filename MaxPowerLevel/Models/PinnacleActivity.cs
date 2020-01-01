using System.Collections.Generic;

namespace MaxPowerLevel.Models
{
    public class PinnacleActivity
    {
        public string Name { get; }

        public IEnumerable<IEnumerable<ItemSlot.SlotHashes>> Encounters { get; }

        public PinnacleActivity(string name, IEnumerable<IEnumerable<ItemSlot.SlotHashes>> encounters)
        {
            Name = name;
            Encounters = encounters;
        }

        public override string ToString() => Name;
    }
}