using System.Collections.Generic;
using System.Linq;
using Destiny2;
using MaxPowerLevel.Models;
using VendorEngrams;

namespace MaxPowerLevel.Services.YearFour
{
    public abstract class Year4Recommendations : AbstractRecommendations
    {
        protected override int TargetRankPlus20Power => 200;

        protected Year4Recommendations(IManifest manifest, IVendorEngramsClient vendorEngrams,
            SeasonPass seasonPass)
            : base(manifest, vendorEngrams, seasonPass)
        {
        }

        protected override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            // TODO: Add pinnacle activities once they are known
            return Enumerable.Empty<PinnacleActivity>();
        }

        // TODO: Verify weak pinnacles are the same as during year 3
        protected override IEnumerable<PinnacleActivity> CreateWeakPinnacleActivities()
            => PinnacleActivities.StandardActivities;
    }
}