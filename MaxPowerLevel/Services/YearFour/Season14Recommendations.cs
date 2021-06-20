using System.Collections.Generic;
using System.Linq;
using Destiny2;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services.YearFour
{
    public class Season14Recommendations : Year4Recommendations
    {
        public Season14Recommendations(IManifest manifest, SeasonPass seasonPass) : base(manifest, seasonPass)
        {
        }

        protected override int SoftCap => 1260;

        protected override int PowerfulCap => 1310;

        protected override int HardCap => 1320;

        protected override uint SeasonHash => 2809059429;

        protected override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            return base.CreatePinnacleActivities().Concat(new[]
            {
                _vaultOfGlass,
                _pressage,
                new PinnacleActivity("Corrupted Conflux Chests", new[] { PinnacleActivities.AllSlots }),
            });
        }

        protected override IEnumerable<PinnacleActivity> CreateWeakPinnacleActivities()
        {
            return base.CreateWeakPinnacleActivities().Concat(new[]
            {
                new PinnacleActivity("Override Conflux Chests", new[] { PinnacleActivities.AllSlots }),
                new PinnacleActivity("Splicer Servitor Bounties", new[] { PinnacleActivities.AllSlots }),
            });
        }
    }
}