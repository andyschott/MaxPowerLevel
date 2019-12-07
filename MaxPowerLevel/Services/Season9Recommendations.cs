using System;

namespace MaxPowerLevel.Services
{
    public class Season9Recommendations : AbstractRecommendations
    {
        protected override int SoftCap => 900;

        protected override int PowerfulCap => 960;

        protected override int HardCap => 970;
    }
}
