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

        public static readonly PinnacleActivity Strikes =
            new PinnacleActivity("Weekly Vanguard Strikes", new[] { AllSlots });
        public static readonly PinnacleActivity Crucible =
            new PinnacleActivity("Crucible Playlist Challenge", new[] { AllSlots });
        public static readonly PinnacleActivity Gambit =
            new PinnacleActivity("Gambit", new[] { AllSlots });
        public static readonly PinnacleActivity Clan =
            new PinnacleActivity("Clan Rewards", new[] { AllSlots });
        public static readonly PinnacleActivity NightfallScore =
            new PinnacleActivity("Nightfall: The Ordeal Weekly Score", new[] { PinnacleActivities.AllSlots });

        public static readonly PinnacleActivity Prophecy = new PinnacleActivity("Prophecy", new[]
        {
            new[]
            {
                ItemSlot.SlotHashes.Helmet,
                ItemSlot.SlotHashes.Gauntlet,
                ItemSlot.SlotHashes.ChestArmor,
                ItemSlot.SlotHashes.LegArmor,
                ItemSlot.SlotHashes.ClassArmor,
            }
        });
    }
}