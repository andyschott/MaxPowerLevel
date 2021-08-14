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

        public override int SoftCap => 1000;

        public override int PowerfulCap => 1050;

        public override int HardCap => 1060;

        public override uint SeasonHash => 248573323;

        public override DateTime? EndDateOverride => new DateTime(2020, 11, 10, 17, 0, 0, DateTimeKind.Utc);

        public override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            return base.CreatePinnacleActivities()
                .Concat(new[]
                {
                    Prophecy,
                    new PinnacleActivity(DarkTimes, new[] { AllSlots })
                });
        }
    }
}