namespace MaxPowerLevel.Services
{
    public class Season8Recommendations : AbstractRecommendations
    {
        protected override int SoftCap => 900;

        protected override int PowerfulCap => 950;

        protected override int HardCap => 960;
    }
}