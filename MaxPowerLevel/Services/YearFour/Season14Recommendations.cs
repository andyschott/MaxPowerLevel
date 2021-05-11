using Destiny2;

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
    }
}