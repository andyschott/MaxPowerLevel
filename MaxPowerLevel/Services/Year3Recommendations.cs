using System.Collections.Generic;
using Destiny2;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services
{
    public abstract class Year3Recommendations : AbstractRecommendations
    {
        protected override int SoftCap => 900;

        protected Year3Recommendations(IManifest manifest) : base(manifest)
        {
        }

        protected override void PopulatePinnacleRecommendations(IDictionary<ItemSlot.SlotHashes, IList<string>> pinnacleRecommendations)
        {
            // Pit of Heresy can drop armor
            pinnacleRecommendations[ItemSlot.SlotHashes.Helmet].Add("Pit of Heresy");
            pinnacleRecommendations[ItemSlot.SlotHashes.Gauntlet].Add("Pit of Heresy");
            pinnacleRecommendations[ItemSlot.SlotHashes.ChestArmor].Add("Pit of Heresy");
            pinnacleRecommendations[ItemSlot.SlotHashes.LegArmor].Add("Pit of Heresy");
            pinnacleRecommendations[ItemSlot.SlotHashes.ClassArmor].Add("Pit of Heresy");

            // Nightmare Hunt: Master can drop anything
            pinnacleRecommendations[ItemSlot.SlotHashes.Kinetic].Add("Nightmare Hunt: Master");
            pinnacleRecommendations[ItemSlot.SlotHashes.Energy].Add("Nightmare Hunt: Master");
            pinnacleRecommendations[ItemSlot.SlotHashes.Power].Add("Nightmare Hunt: Master");
            pinnacleRecommendations[ItemSlot.SlotHashes.Helmet].Add("Nightmare Hunt: Master");
            pinnacleRecommendations[ItemSlot.SlotHashes.Gauntlet].Add("Nightmare Hunt: Master");
            pinnacleRecommendations[ItemSlot.SlotHashes.ChestArmor].Add("Nightmare Hunt: Master");
            pinnacleRecommendations[ItemSlot.SlotHashes.LegArmor].Add("Nightmare Hunt: Master");
            pinnacleRecommendations[ItemSlot.SlotHashes.ClassArmor].Add("Nightmare Hunt: Master");

            // Nightfall: The Ordeal Weekly Score can drop anything
            pinnacleRecommendations[ItemSlot.SlotHashes.Kinetic].Add("Nightfall: The Ordeal Weekly Score");
            pinnacleRecommendations[ItemSlot.SlotHashes.Energy].Add("Nightfall: The Ordeal Weekly Score");
            pinnacleRecommendations[ItemSlot.SlotHashes.Power].Add("Nightfall: The Ordeal Weekly Score");
            pinnacleRecommendations[ItemSlot.SlotHashes.Helmet].Add("Nightfall: The Ordeal Weekly Score");
            pinnacleRecommendations[ItemSlot.SlotHashes.Gauntlet].Add("Nightfall: The Ordeal Weekly Score");
            pinnacleRecommendations[ItemSlot.SlotHashes.ChestArmor].Add("Nightfall: The Ordeal Weekly Score");
            pinnacleRecommendations[ItemSlot.SlotHashes.LegArmor].Add("Nightfall: The Ordeal Weekly Score");
            pinnacleRecommendations[ItemSlot.SlotHashes.ClassArmor].Add("Nightfall: The Ordeal Weekly Score");

            // Garden of Salvation can drop everything except Power
            pinnacleRecommendations[ItemSlot.SlotHashes.Kinetic].Add("Garden of Salvation");
            pinnacleRecommendations[ItemSlot.SlotHashes.Energy].Add("Garden of Salvation");
            pinnacleRecommendations[ItemSlot.SlotHashes.Helmet].Add("Garden of Salvation");
            pinnacleRecommendations[ItemSlot.SlotHashes.Gauntlet].Add("Garden of Salvation");
            pinnacleRecommendations[ItemSlot.SlotHashes.ChestArmor].Add("Garden of Salvation");
            pinnacleRecommendations[ItemSlot.SlotHashes.LegArmor].Add("Garden of Salvation");
            pinnacleRecommendations[ItemSlot.SlotHashes.ClassArmor].Add("Garden of Salvation");
        }
    }
}