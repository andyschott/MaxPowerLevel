using System.Collections.Generic;
using System.Linq;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services.YearFive
{
    public class Season17 : AbstractYear5Season
    {
        public override int SoftCap => 1510;

        public override int PowerfulCap => 1560;

        public override int HardCap => 1570;

        public override uint SeasonHash => 2809059432;
    }
}
