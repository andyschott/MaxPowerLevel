using System.Collections.Generic;
using System.Linq;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services.YearFour
{
    public class Season13 : AbstractYear4Season
    {
        public override int SoftCap => 1250;

        public override int PowerfulCap => 1300;

        public override int HardCap => 1310;

        public override uint SeasonHash => 2809059426;

        protected override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            return base.CreatePinnacleActivities().Concat(new[]
            {
                _deepStoneCrypt,
                _pressage
            });
        }
    }
}