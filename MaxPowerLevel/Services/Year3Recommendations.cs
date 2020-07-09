using System.Collections.Generic;
using Destiny2;
using MaxPowerLevel.Models;
using VendorEngrams;

namespace MaxPowerLevel.Services
{
    public abstract class Year3Recommendations : AbstractRecommendations
    {
        protected override int SoftCap => 900;
        protected override int TargetRankPlus20Power => 200;

        private const string Nightfall = "Nightfall: The Ordeal Weekly Score";
        private const string MasterNightmareHunt = "Nightmare Hunt: Master";
        private const string PitOfHeresy = "Pit of Heresy";
        private const string GardenOfSalvation = "Garden of Salvation";
        private const string Strikes = "Weekly Vanguard Strikes";
        private const string Crucible = "Crucible Core Playlist Challenge";
        private const string Gambit = "Weekly Gambit Challenge";
        private const string Clan = "Clan Rewards";

        protected static readonly ItemSlot.SlotHashes[] _allSlots = new[]
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
        
        protected Year3Recommendations(IManifest manifest, IVendorEngramsClient vendorEngrams,
            SeasonPass seasonPass)
            : base(manifest, vendorEngrams, seasonPass)
        {
        }

        protected override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
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
                new PinnacleActivity(MasterNightmareHunt, new[] { _allSlots }),
                // Nightfall: The Ordeal Weekly Score can drop anything
                new PinnacleActivity(Nightfall, new[] { _allSlots }),
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

        protected override IEnumerable<PinnacleActivity> CreateWeakPinnacleActivities()
        {
            return new[]
            {
                new PinnacleActivity(Strikes, new[] { _allSlots }),
                new PinnacleActivity(Crucible, new[] { _allSlots }),
                new PinnacleActivity(Gambit, new[] { _allSlots }),
                new PinnacleActivity(Clan, new[] { _allSlots })
            };
        }
    }
}