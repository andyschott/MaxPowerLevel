using System;
using System.Collections.Generic;
using System.Linq;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services
{
    public abstract class AbstractSeason : ISeason
    {
        public abstract int SoftCap { get; }

        public abstract int PowerfulCap { get; }

        public abstract int HardCap { get; }

        public abstract uint SeasonHash { get; }

        public virtual int TargetRankPlus20Power => 200;

        public virtual DateTime? EndDateOverride => null;

        public IEnumerable<PinnacleActivity> CreatePinnacleActivities(bool includeTrials)
        {
            var pinnacleActivities = CreatePinnacleActivities();
            if(includeTrials)
            {
                pinnacleActivities = pinnacleActivities.Concat(new[]
                {
                    TrialsRoundWins,
                    TrialsWins,
                });
            }

            return pinnacleActivities;
        }

        protected abstract IEnumerable<PinnacleActivity> CreatePinnacleActivities();
        public virtual IEnumerable<PinnacleActivity> CreateWeakPinnacleActivities() => new[]
        {
            Strikes,
            Crucible,
            Gambit,
            Clan,
        };

        protected static readonly ItemSlot.SlotHashes[] AllSlots = new[]
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

        protected static readonly PinnacleActivity Strikes =
            new PinnacleActivity("Weekly Vanguard Strikes", new[] { AllSlots });
        protected static readonly PinnacleActivity Crucible =
            new PinnacleActivity("Crucible Playlist Challenge", new[] { AllSlots });
        protected static readonly PinnacleActivity Gambit =
            new PinnacleActivity("Gambit", new[] { AllSlots });
        protected static readonly PinnacleActivity Clan =
            new PinnacleActivity("Clan Rewards", new[] { AllSlots });
        protected static readonly PinnacleActivity NightfallScore =
            new PinnacleActivity("Nightfall: The Ordeal Weekly Score", new[] { AllSlots });
        protected static readonly PinnacleActivity TrialsRoundWins =
            new PinnacleActivity("Trials of Osiris (Round Wins)", new[] { AllSlots });
        protected static readonly PinnacleActivity TrialsWins =
            new PinnacleActivity("Trials of Osiris (Match Wins)", new[] { AllSlots });

        protected static readonly PinnacleActivity Prophecy = new PinnacleActivity("Prophecy", new[]
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