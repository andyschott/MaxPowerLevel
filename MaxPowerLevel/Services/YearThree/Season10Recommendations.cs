using System;
using Destiny2;

namespace MaxPowerLevel.Services.YearThree
{
    public class Season10Recommendations : Year3Recommendations
    {
        public override int SoftCap => 950;
        public override int PowerfulCap => 1000;
        public override int HardCap => 1010;
        public override uint SeasonHash => 4035491417;

        // Season 10 actually ends on June 9
        public override DateTime? EndDateOverride => new DateTime(2020, 6, 9, 17, 0, 0, DateTimeKind.Utc);
    }
}