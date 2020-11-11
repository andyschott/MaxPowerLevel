using Destiny2;

namespace MaxPowerLevel.Services.YearFour
{
    public class Season12Recommendations : Year4Recommendations
    {
        public Season12Recommendations(IManifest manifest, SeasonPass seasonPass) :
            base(manifest, seasonPass)
        {
        }

        protected override int SoftCap => 1200;

        protected override int PowerfulCap => 1250;

        protected override int HardCap => 1260;

        protected override uint SeasonHash => 2809059427;
    }
}