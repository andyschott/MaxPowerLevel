using System;
using System.Collections.Generic;
using System.Linq;
using Destiny2;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services.YearFour
{
    public class Season12Recommendations : Year4Recommendations
    {
        public Season12Recommendations(IManifest manifest, SeasonPass seasonPass) :
            base(manifest, seasonPass)
        {
        }

        protected override int SoftCap => 1200;

        protected override int PowerfulCap => 1250;

        protected override int HardCap => 1260;

        protected override uint SeasonHash => 2809059427;
        protected override DateTime? EndDateOverride => new DateTime(2021, 2, 9, 17, 0, 0, DateTimeKind.Utc);
        protected override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            var year4Pinnacles = base.CreatePinnacleActivities();
            return year4Pinnacles.Concat(new[]
            {
                new PinnacleActivity("Weekly Wrathborn Hunts", new[] { PinnacleActivities.AllSlots }),
                new PinnacleActivity("Coup Dê Grace", new[] { PinnacleActivities.AllSlots }),
                _deepStoneCrypt,
                _pressage
            });
        }
    }
}