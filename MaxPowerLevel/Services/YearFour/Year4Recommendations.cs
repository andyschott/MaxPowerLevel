using System.Collections.Generic;
using Destiny2;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services.YearFour
{
    public abstract class Year4Recommendations : AbstractRecommendations
    {
        protected override int TargetRankPlus20Power => 200;

        protected Year4Recommendations(IManifest manifest, SeasonPass seasonPass)
            : base(manifest, seasonPass)
        {
        }

        protected override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            return new[]
            {
                PinnacleActivities.NightfallScore,
                new PinnacleActivity("Deep Stone Crypt", 541780856, new[]
                {
                    new[]
                    {
                        ItemSlot.SlotHashes.Gauntlet,
                        ItemSlot.SlotHashes.ClassArmor,
                        ItemSlot.SlotHashes.LegArmor,
                        ItemSlot.SlotHashes.Energy // Scout Rifle
                    },
                    new[]
                    {
                        ItemSlot.SlotHashes.Gauntlet,
                        ItemSlot.SlotHashes.ClassArmor,
                        ItemSlot.SlotHashes.LegArmor,
                        ItemSlot.SlotHashes.Kinetic, // Sniper Rifle and Shotgun
                    },
                    new[]
                    {
                        ItemSlot.SlotHashes.ChestArmor,
                        ItemSlot.SlotHashes.Gauntlet,
                        ItemSlot.SlotHashes.ClassArmor,
                        ItemSlot.SlotHashes.Energy // Hand Cannon
                    },
                    new[]
                    {
                        ItemSlot.SlotHashes.Helmet,
                        ItemSlot.SlotHashes.ChestArmor,
                        ItemSlot.SlotHashes.LegArmor,
                        ItemSlot.SlotHashes.Power // Sword and HMG
                    },
                }),
                new PinnacleActivity("Weekly Exo Challenge", 1713200903, new[] { PinnacleActivities.AllSlots }),
                new PinnacleActivity("Weekly Empire Hunts", 291895718, new[] { PinnacleActivities.AllSlots }),
                PinnacleActivities.Prophecy,
                new PinnacleActivity("Harbinger",1086730368, new[] { PinnacleActivities.AllSlots }),

            };
        }

        protected override IEnumerable<PinnacleActivity> CreateWeakPinnacleActivities() => new[]
        {
            PinnacleActivities.Strikes,
            PinnacleActivities.Crucible,
            PinnacleActivities.Gambit,
            PinnacleActivities.Clan,
        };
    }
}