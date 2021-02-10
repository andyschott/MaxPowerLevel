using Destiny2;

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
    }
}