using System;
using System.Collections.Generic;
using System.Linq;
using Destiny2;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services.YearThree
{
    public class Season11Recommendations : Year3Recommendations
    {
        private const string DarkTimes = "Dark Times"; // Weekly for PE completions

        public Season11Recommendations(IManifest manifest, SeasonPass seasonPass)
            : base(manifest, seasonPass)
        {
        }

        protected override int SoftCap => 1000;

        protected override int PowerfulCap => 1050;

        protected override int HardCap => 1060;

        protected override uint SeasonHash => 248573323;

        protected override DateTime? EndDateOverride => new DateTime(2020, 11, 10, 17, 0, 0, DateTimeKind.Utc);

        protected override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            return base.CreatePinnacleActivities()
                .Concat(new[]
                {
                    PinnacleActivities.Prophecy,
                    new PinnacleActivity(DarkTimes, uint.MinValue, new[] { PinnacleActivities.AllSlots }) //can't find the hash
                });
        }
    }
}