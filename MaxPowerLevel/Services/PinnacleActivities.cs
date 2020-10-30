using System.Collections.Generic;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services
{
    static class PinnacleActivities
    {
        public static readonly ItemSlot.SlotHashes[] AllSlots = new[]
        {
            ItemSlot.SlotHashes.Kinetic,
            ItemSlot.SlotHashes.Energy,
            ItemSlot.SlotHashes.Power,
            ItemSlot.SlotHashes.Helmet,
            ItemSlot.SlotHashes.Gauntlet,
            ItemSlot.SlotHashes.ChestArmor,
            ItemSlot.SlotHashes.LegArmor,
            ItemSlot.SlotHashes.ClassArmor,
        };

        public static readonly IEnumerable<PinnacleActivity> StandardActivities =
            new[]
            {
                new PinnacleActivity("Weekly Vanguard Strikes", new[] { AllSlots }),
                new PinnacleActivity("Crucible Core Playlist Challenge", new[] { AllSlots }),
                new PinnacleActivity("Weekly Gambit Challenge", new[] { AllSlots }),
                new PinnacleActivity("Clan Rewards", new[] { AllSlots })
            };
    }
}