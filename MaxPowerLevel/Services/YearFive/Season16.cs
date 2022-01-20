using System;
using System.Collections.Generic;
using System.Linq;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services.YearFive
{
    public class Season16 : AbstractYear5Season
    {
        public override int SoftCap => 1500;

        public override int PowerfulCap => 1550;

        public override int HardCap => 1560;

        public override uint SeasonHash => 2809059431;

        // TODO: Verify this once the actual manifest is out
        public override DateTime? EndDateOverride => new DateTime(2022, 5, 17, 17, 0, 0, DateTimeKind.Utc);

        protected override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            // TODO: Populate once new pinnacle activities are known
            return Enumerable.Empty<PinnacleActivity>();
        }
    }
}