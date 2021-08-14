using System.Collections.Generic;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services.YearThree
{
    public abstract class AbstractYear3Season : AbstractSeason
    {
        public override int SoftCap => 900;

        private const string MasterNightmareHunt = "Nightmare Hunt: Master";
        private const string PitOfHeresy = "Pit of Heresy";
        private const string GardenOfSalvation = "Garden of Salvation";
        
        public override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            return new[]
            {
                // Pit of Heresy can drop armor
                new PinnacleActivity(PitOfHeresy, new[]
                {
                    new[]
                    {
                        ItemSlot.SlotHashes.Helmet,
                        ItemSlot.SlotHashes.Gauntlet,
                        ItemSlot.SlotHashes.ChestArmor,
                        ItemSlot.SlotHashes.LegArmor,
                        ItemSlot.SlotHashes.ClassArmor,
                    }
                }),
                // Nightmare Hunt: Master can drop anything
                new PinnacleActivity(MasterNightmareHunt, new[] { AllSlots }),
                // Nightfall: The Ordeal Weekly Score can drop anything
                NightfallScore,
                // Garden of Salvation
                new PinnacleActivity(GardenOfSalvation, new[]
                {
                    // Encounter 1 drops Bow (Kinetic), Fusion Rifle (Energy), Boots
                    new[]
                    {
                        ItemSlot.SlotHashes.Kinetic,
                        ItemSlot.SlotHashes.Energy,
                        ItemSlot.SlotHashes.LegArmor
                    },
                    // Encounter 2 drops Auto Rifle (Energy), Shotgun (Energy), Arms
                    new[]
                    {
                        ItemSlot.SlotHashes.Energy,
                        ItemSlot.SlotHashes.Energy,
                        ItemSlot.SlotHashes.Gauntlet
                    },
                    // Encounter 3 drops Pulse Rifle (Kinetic), Hand Cannon (Energy), Chest
                    new[]
                    {
                        ItemSlot.SlotHashes.Kinetic,
                        ItemSlot.SlotHashes.Energy,
                        ItemSlot.SlotHashes.ChestArmor
                    },
                    // Encounter 3 drops Sniper Rifle (Energy), Helmet, Class Item
                    new[]
                    {
                        ItemSlot.SlotHashes.Energy,
                        ItemSlot.SlotHashes.Helmet,
                        ItemSlot.SlotHashes.ClassArmor
                    }
                })
            };
        }
    }
}
