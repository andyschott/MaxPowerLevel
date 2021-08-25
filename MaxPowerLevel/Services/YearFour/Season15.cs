using System.Collections.Generic;
using System.Linq;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services.YearFour
{
    public class Season15 : AbstractYear4Season
    {
        public override int SoftCap => 1270;

        public override int PowerfulCap => 1320;

        public override int HardCap => 1330;

        public override uint SeasonHash => 2698636901;

        public override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            return base.CreatePinnacleActivities().Concat(new[]
            {
                new PinnacleActivity("Shattered Realm", new[] { AllSlots })
            });
        }
    }
}