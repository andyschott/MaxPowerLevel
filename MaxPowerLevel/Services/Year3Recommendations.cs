using System.Collections.Generic;
using Destiny2;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services
{
    public abstract class Year3Recommendations : AbstractRecommendations
    {
        protected override int SoftCap => 900;

        private const string Nightfall = "Nightfall: The Ordeal Weekly Score";
        private const string MasterNightmareHunt = "Nightmare Hunt: Master";
        private const string PitOfHeresy = "Pit of Heresy";
        private const string GardenOfSalvation = "Garden of Salvation";

        protected Year3Recommendations(IManifest manifest) : base(manifest)
        {
        }

        protected override void PopulatePinnacleActivities(IDictionary<string, ISet<ItemSlot.SlotHashes>> pinnacleRecommendations)
        {
            // Pit of Heresy can drop armor
            pinnacleRecommendations.Add(PitOfHeresy, new HashSet<ItemSlot.SlotHashes>(new[]
            {
                ItemSlot.SlotHashes.Helmet,
                ItemSlot.SlotHashes.Gauntlet,
                ItemSlot.SlotHashes.ChestArmor,
                ItemSlot.SlotHashes.LegArmor,
                ItemSlot.SlotHashes.ClassArmor,
            }));

            // Nightmare Hunt: Master can drop anything
            pinnacleRecommendations.Add(MasterNightmareHunt, new HashSet<ItemSlot.SlotHashes>(new[]
            {
                ItemSlot.SlotHashes.Kinetic,
                ItemSlot.SlotHashes.Energy,
                ItemSlot.SlotHashes.Power,
                ItemSlot.SlotHashes.Helmet,
                ItemSlot.SlotHashes.Gauntlet,
                ItemSlot.SlotHashes.ChestArmor,
                ItemSlot.SlotHashes.LegArmor,
                ItemSlot.SlotHashes.ClassArmor,
            }));

            // Nightfall: The Ordeal Weekly Score can drop anything
            pinnacleRecommendations.Add(Nightfall, new HashSet<ItemSlot.SlotHashes>(new[]
            {
                ItemSlot.SlotHashes.Kinetic,
                ItemSlot.SlotHashes.Energy,
                ItemSlot.SlotHashes.Power,
                ItemSlot.SlotHashes.Helmet,
                ItemSlot.SlotHashes.Gauntlet,
                ItemSlot.SlotHashes.ChestArmor,
                ItemSlot.SlotHashes.LegArmor,
                ItemSlot.SlotHashes.ClassArmor,
            }));

            // Garden of Salvation can drop everything except Power
            pinnacleRecommendations.Add(GardenOfSalvation, new HashSet<ItemSlot.SlotHashes>(new[]
            {
                ItemSlot.SlotHashes.Kinetic,
                ItemSlot.SlotHashes.Energy,
                ItemSlot.SlotHashes.Helmet,
                ItemSlot.SlotHashes.Gauntlet,
                ItemSlot.SlotHashes.ChestArmor,
                ItemSlot.SlotHashes.LegArmor,
                ItemSlot.SlotHashes.ClassArmor,
            }));
        }
    }
}