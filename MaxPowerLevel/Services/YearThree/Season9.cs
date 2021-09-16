using System.Collections.Generic;
using System.Linq;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services.YearThree
{
    public class Season9 : AbstractYear3Season
    {
        public override int PowerfulCap => 960;
        public override int HardCap => 970;
        public override uint SeasonHash => 2007338097;

        private const string SundialLegend = "The Sundial: Legend";

        protected override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
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
