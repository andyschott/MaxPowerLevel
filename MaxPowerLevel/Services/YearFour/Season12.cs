using System;
using System.Collections.Generic;
using System.Linq;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services.YearFour
{
    public class Season12 : AbstractYear4Season
    {
        public override int SoftCap => 1200;

        public override int PowerfulCap => 1250;

        public override int HardCap => 1260;

        public override uint SeasonHash => 2809059427;
        public override DateTime? EndDateOverride => new DateTime(2021, 2, 9, 17, 0, 0, DateTimeKind.Utc);
        public override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            var year4Pinnacles = base.CreatePinnacleActivities();
            return year4Pinnacles.Concat(new[]
            {
                new PinnacleActivity("Weekly Wrathborn Hunts", new[] { AllSlots }),
                new PinnacleActivity("Coup Dê Grace", new[] { AllSlots }),
                _deepStoneCrypt,
                _pressage
            });
        }
    }
}