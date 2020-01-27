using Destiny2;

namespace MaxPowerLevel.Services
{
    public class Season8Recommendations : Year3Recommendations
    {
        protected override int PowerfulCap => 950;

        protected override int HardCap => 960;
        protected override uint SeasonHash => 3612906877;
        protected override int TargetRankPlus20Power => 201;

        public Season8Recommendations(IManifest manifest)
            : base(manifest)
        {
        }
    }
}