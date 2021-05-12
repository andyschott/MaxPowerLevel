using System.Collections.Generic;
using System.Linq;
using Destiny2;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services.YearFour
{
    public class Season13Recommendations : Year4Recommendations
    {
        public Season13Recommendations(IManifest manifest, SeasonPass seasonPass)
            : base(manifest, seasonPass)
        {
        }

        protected override int SoftCap => 1250;

        protected override int PowerfulCap => 1300;

        protected override int HardCap => 1310;

        protected override uint SeasonHash => 2809059426;

        protected override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            return base.CreatePinnacleActivities().Concat(new[]
            {
                _deepStoneCrypt,
                _pressage
            });
        }
    }
}