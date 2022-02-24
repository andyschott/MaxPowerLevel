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

        protected override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            return base.CreatePinnacleActivities().Concat(new[]
            {
                new PinnacleActivity("For the Light...Against the Light", new[] { AllSlots })
            });
        }
    }
}