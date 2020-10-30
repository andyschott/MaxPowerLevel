using System;
using Destiny2;

namespace MaxPowerLevel.Services.YearThree
{
    public class Season10Recommendations : Year3Recommendations
    {
        protected override int SoftCap => 950;
        protected override int PowerfulCap => 1000;
        protected override int HardCap => 1010;
        protected override uint SeasonHash => 4035491417;

        // Season 10 actually ends on June 9
        protected override DateTime? EndDateOverride => new DateTime(2020, 6, 9, 17, 0, 0, DateTimeKind.Utc);

        public Season10Recommendations(IManifest manifest, SeasonPass seasonPass)
            : base(manifest, seasonPass)
        {
        }
    }
}