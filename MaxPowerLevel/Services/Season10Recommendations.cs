using Destiny2;

namespace MaxPowerLevel.Services
{
    public class Season10Recommendations : Year3Recommendations
    {
        protected override int SoftCap => 950;
        protected override int PowerfulCap => 1000;
        protected override int HardCap => 1010;
        protected override uint SeasonHash => 4035491417;

        public Season10Recommendations(IManifest manifest) : base(manifest)
        {
        }
    }
}