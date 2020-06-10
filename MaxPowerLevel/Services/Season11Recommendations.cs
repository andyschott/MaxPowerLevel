using System.Collections.Generic;
using System.Linq;
using Destiny2;
using MaxPowerLevel.Models;
using VendorEngrams;

namespace MaxPowerLevel.Services
{
    public class Season11Recommendations : Year3Recommendations
    {
        private const string Prophecy = "Prophecy"; // New Dungeon
        private const string DarkTimes = "Dark Times"; // Weekly for PE completions

        public Season11Recommendations(IManifest manifest, IVendorEngramsClient vendorEngrams, SeasonPass seasonPass)
            : base(manifest, vendorEngrams, seasonPass)
        {
        }

        protected override int SoftCap => 1000;

        protected override int PowerfulCap => 1050;

        protected override int HardCap => 1060;

        protected override uint SeasonHash => 248573323;

        protected override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            return base.CreatePinnacleActivities()
                .Concat(new[]
                {
                    new PinnacleActivity(Prophecy, new[] { _allSlots }),
                    new PinnacleActivity(DarkTimes, new[] { _allSlots })
                });
        }
    }
}