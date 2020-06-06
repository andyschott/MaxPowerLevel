using Destiny2;
using VendorEngrams;

namespace MaxPowerLevel.Services
{
    public class Season11Recommendations : Year3Recommendations
    {
        public Season11Recommendations(IManifest manifest, IVendorEngramsClient vendorEngrams, SeasonPass seasonPass)
            : base(manifest, vendorEngrams, seasonPass)
        {
        }

        protected override int SoftCap => 1000;

        protected override int PowerfulCap => 1050;

        protected override int HardCap => 1060;

        protected override uint SeasonHash => 248573323;
    }
}