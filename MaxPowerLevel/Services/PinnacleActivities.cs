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
            new PinnacleActivity("Weekly Vanguard Strikes", 1437935813, new[] { AllSlots });
        public static readonly PinnacleActivity Crucible =
            new PinnacleActivity("Crucible Playlist Challenge", 3312774044, new[] { AllSlots });
        public static readonly PinnacleActivity Gambit =
            new PinnacleActivity("Gambit", 3448738070, new[] { AllSlots });
        public static readonly PinnacleActivity Clan =
            new PinnacleActivity("Clan Rewards", 3603098564, new[] { AllSlots });
        public static readonly PinnacleActivity NightfallScore =
            new PinnacleActivity("Nightfall: The Ordeal Weekly Score", 2029743966, new[] { PinnacleActivities.AllSlots });

        public static readonly PinnacleActivity Prophecy = new PinnacleActivity("Prophecy", 825965416, new[]
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