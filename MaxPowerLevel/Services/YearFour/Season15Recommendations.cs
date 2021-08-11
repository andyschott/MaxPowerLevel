using Destiny2;

namespace MaxPowerLevel.Services.YearFour
{
    public class Season15Recommendations : Year4Recommendations
    {
        public Season15Recommendations(IManifest manifest, SeasonPass seasonPass) : base(manifest, seasonPass)
        {
        }

        protected override int SoftCap => 1270;

        protected override int PowerfulCap => 1320;

        protected override int HardCap => 1330;

        protected override uint SeasonHash => 2698636901;
    }
}