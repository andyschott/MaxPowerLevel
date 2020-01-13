using System.Collections.Generic;

namespace MaxPowerLevel.Models
{
    public class PinnacleActivity
    {
        public string Name { get; }

        public ItemSlot.SlotHashes[][] Encounters { get; }

        public PinnacleActivity(string name, ItemSlot.SlotHashes[][] encounters)
        {
            Name = name;
            Encounters = encounters;
        }

        public override string ToString() => Name;
    }
}