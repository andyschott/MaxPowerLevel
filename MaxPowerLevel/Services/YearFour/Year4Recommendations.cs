using System.Collections.Generic;
using Destiny2;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services.YearFour
{
    public abstract class Year4Recommendations : AbstractRecommendations
    {
        protected override int TargetRankPlus20Power => 200;

        protected static readonly PinnacleActivity _pressage = new PinnacleActivity("Pressage", new[] { PinnacleActivities.AllSlots });
        protected static readonly PinnacleActivity _deepStoneCrypt = new PinnacleActivity("Deep Stone Crypt", new[]
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
        });

        protected static readonly PinnacleActivity _vaultOfGlass = new PinnacleActivity("Vault of Glass", new[]
        {
            new[]
            {
                ItemSlot.SlotHashes.Gauntlet,
                ItemSlot.SlotHashes.ClassArmor,
                ItemSlot.SlotHashes.Energy, // Scout Rifle, Shotgun
                ItemSlot.SlotHashes.Power // Machine Gun
            },
            new[]
            {
                ItemSlot.SlotHashes.Gauntlet,
                ItemSlot.SlotHashes.LegArmor,
                ItemSlot.SlotHashes.Kinetic, // Sniper Rifle
                ItemSlot.SlotHashes.Energy, // Scout Rifle, Shotgun
            },
            new[]
            {
                ItemSlot.SlotHashes.ChestArmor,
                ItemSlot.SlotHashes.Gauntlet,
                ItemSlot.SlotHashes.Kinetic, // Hand Cannon
                ItemSlot.SlotHashes.Energy, // Scout Rifle, Shotgun
                ItemSlot.SlotHashes.Power // Machine Gun
            },
            new[]
            {
                ItemSlot.SlotHashes.Helmet,
                ItemSlot.SlotHashes.LegArmor,
                ItemSlot.SlotHashes.Kinetic, // Hand Cannon
                ItemSlot.SlotHashes.Energy, // Scout Rifle, Shotgun
                ItemSlot.SlotHashes.Power // Rocket Launcher
            },
            new[]
            {
                ItemSlot.SlotHashes.Helmet,
                ItemSlot.SlotHashes.ChestArmor,
                ItemSlot.SlotHashes.Kinetic, // Sniper Rifle
                ItemSlot.SlotHashes.Power // Machine Gun, Rocket Launcher
            }
        });
        protected Year4Recommendations(IManifest manifest, SeasonPass seasonPass)
            : base(manifest, seasonPass)
        {
        }

        protected override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            return new[]
            {
                PinnacleActivities.NightfallScore,
                new PinnacleActivity("Weekly Exo Challenge", new[] { PinnacleActivities.AllSlots }),
                new PinnacleActivity("Weekly Empire Hunts", new[] { PinnacleActivities.AllSlots }),
                PinnacleActivities.Prophecy,
                new PinnacleActivity("Harbringer", new[] { PinnacleActivities.AllSlots }),
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