using System.Collections.Generic;
using System.Linq;
using Destiny2;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services.YearThree
{
    public class Season9Recommendations : Year3Recommendations
    {
        protected override int PowerfulCap => 960;
        protected override int HardCap => 970;
        protected override uint SeasonHash => 2007338097;

        private const string SundialLegend = "The Sundial: Legend";

        public Season9Recommendations(IManifest manifest, SeasonPass seasonPass)
            : base(manifest, seasonPass)
        {
        }

        protected override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            return base.CreatePinnacleActivities()
                .Concat(new[]
                {
                    new PinnacleActivity(SundialLegend, uint.MinValue, new[] //can't find the hash for sundial
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
