using System.Collections.Generic;

namespace MaxPowerLevel.Models
{
    public class PinnacleActivity
    {
        public string Name { get; }

        public uint MilestoneHash { get; }

        public ItemSlot.SlotHashes[][] Encounters { get; }

        public PinnacleActivity(string name, uint milestonHash, ItemSlot.SlotHashes[][] encounters)
        {
            Name = name;
            MilestoneHash = milestonHash;
            Encounters = encounters;
        }

        public override string ToString() => Name;
    }
}