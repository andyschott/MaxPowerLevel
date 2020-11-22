using System.Collections.Generic;
using System.Linq;
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
                new PinnacleActivity("Deep Stone Crypt", new[]
                {
                    // Deep Stone Crypt has 4 encounters. Update drops for each encounter once the loot table is known
                    PinnacleActivities.AllSlots,
                    PinnacleActivities.AllSlots,
                    PinnacleActivities.AllSlots,
                    PinnacleActivities.AllSlots,
                }),
                new PinnacleActivity("Weekly Wrathborn Hunts", new[] { PinnacleActivities.AllSlots }),
                new PinnacleActivity("Weekly Exo Challenge", new[] { PinnacleActivities.AllSlots }),
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