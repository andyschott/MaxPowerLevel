using Destiny2;

namespace MaxPowerLevel.Services
{
    public class Season9Recommendations : AbstractRecommendations
    {
        protected override int SoftCap => 900;

        protected override int PowerfulCap => 960;

        protected override int HardCap => 970;
        protected override uint SeasonHash => 2007338097;

        public Season9Recommendations(IManifest manifest)
            : base(manifest)
        {
        }
    }
}
