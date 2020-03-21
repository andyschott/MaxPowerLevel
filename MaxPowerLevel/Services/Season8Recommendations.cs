using Destiny2;
using VendorEngrams;

namespace MaxPowerLevel.Services
{
    public class Season8Recommendations : Year3Recommendations
    {
        protected override int PowerfulCap => 950;

        protected override int HardCap => 960;
        protected override uint SeasonHash => 3612906877;

        public Season8Recommendations(IManifest manifest, IVendorEngramsClient vendorEngrams)
            : base(manifest, vendorEngrams)
        {
        }
    }
}