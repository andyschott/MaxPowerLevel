using Destiny2;

namespace MaxPowerLevel.Services
{
    public class Season8Recommendations : AbstractRecommendations
    {
        protected override int SoftCap => 900;

        protected override int PowerfulCap => 950;

        protected override int HardCap => 960;
        protected override uint SeasonHash => 3612906877;

        public Season8Recommendations(IManifest manifest)
            : base(manifest)
        {
        }
    }
}