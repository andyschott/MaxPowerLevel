using System.Collections.Generic;
using Destiny2;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services.YearThree
{
    public abstract class Year3Recommendations : AbstractRecommendations
    {
        protected override int SoftCap => 900;
        protected override int TargetRankPlus20Power => 200;

        private const string MasterNightmareHunt = "Nightmare Hunt: Master";
        private const string PitOfHeresy = "Pit of Heresy";
        private const string GardenOfSalvation = "Garden of Salvation";
        
        protected Year3Recommendations(IManifest manifest, SeasonPass seasonPass)
            : base(manifest, seasonPass)
        {
        }

        protected override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            return new[]
            {
                // Pit of Heresy can drop armor
                new PinnacleActivity(PitOfHeresy, uint.MinValue, new[] //can't find the hash for PoH
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
                new PinnacleActivity(MasterNightmareHunt, 291895718, new[] { PinnacleActivities.AllSlots }),
                // Nightfall: The Ordeal Weekly Score can drop anything
                PinnacleActivities.NightfallScore,
                // Garden of Salvation
                new PinnacleActivity(GardenOfSalvation, 2712317338, new[]
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

        protected override IEnumerable<PinnacleActivity> CreateWeakPinnacleActivities() => new[]
        {
            PinnacleActivities.Strikes,
            PinnacleActivities.Crucible,
            PinnacleActivities.Gambit,
            PinnacleActivities.Clan
        };
    }
}
