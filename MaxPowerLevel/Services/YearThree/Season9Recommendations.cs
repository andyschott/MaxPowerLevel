using System.Collections.Generic;
using System.Linq;
using Destiny2;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services.YearThree
{
    public class Season9Recommendations : Year3Recommendations
    {
        public override int PowerfulCap => 960;
        public override int HardCap => 970;
        public override uint SeasonHash => 2007338097;

        private const string SundialLegend = "The Sundial: Legend";

        public override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            return base.CreatePinnacleActivities()
                .Concat(new[]
                {
                    new PinnacleActivity(SundialLegend, new[]
                    {
                        new[]
                        {
                            ItemSlot.SlotHashes.Kinetic,
                            ItemSlot.SlotHashes.Energy,
                            ItemSlot.SlotHashes.Power
                        }
                    })
                });
        }
    }
}
