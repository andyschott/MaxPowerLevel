using System.Collections.Generic;
using System.Linq;
using Destiny2;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services.YearFour
{
    public class Season13Recommendations : Year4Recommendations
    {
        public override int SoftCap => 1250;

        public override int PowerfulCap => 1300;

        public override int HardCap => 1310;

        public override uint SeasonHash => 2809059426;

        public override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            return base.CreatePinnacleActivities().Concat(new[]
            {
                _deepStoneCrypt,
                _pressage
            });
        }
    }
}