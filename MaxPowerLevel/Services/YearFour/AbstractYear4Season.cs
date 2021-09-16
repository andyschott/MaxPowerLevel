using System.Collections.Generic;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services.YearFour
{
    public abstract class AbstractYear4Season : AbstractSeason
    {
        protected static readonly PinnacleActivity _pressage = new PinnacleActivity("Pressage", new[] { AllSlots });
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
        protected override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            return new[]
            {
                NightfallScore,
                new PinnacleActivity("Weekly Exo Challenge", new[] { AllSlots }),
                new PinnacleActivity("Weekly Empire Hunts", new[] { AllSlots }),
                Prophecy,
                new PinnacleActivity("Harbringer", new[] { AllSlots }),
            };
        }
    }
}