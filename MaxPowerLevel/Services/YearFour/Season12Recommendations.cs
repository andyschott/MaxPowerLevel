using Destiny2;
using VendorEngrams;

namespace MaxPowerLevel.Services.YearFour
{
    public class Season12Recommendations : Year4Recommendations
    {
        public Season12Recommendations(IManifest manifest, IVendorEngramsClient vendorEngrams,
            SeasonPass seasonPass) : base(manifest, vendorEngrams, seasonPass)
        {
        }

        protected override int SoftCap => 1200;

        protected override int PowerfulCap => 1250;

        protected override int HardCap => 1260;

        protected override uint SeasonHash => 1162944553;
    }
}