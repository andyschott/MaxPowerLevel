using System.Collections.Generic;
using System.Linq;
using Destiny2;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services
{
    public class Season9Recommendations : Year3Recommendations
    {
        protected override int PowerfulCap => 960;
        protected override int HardCap => 970;
        protected override uint SeasonHash => 2007338097;

        private const string SundialLegend = "The Sundial: Legend";

        public Season9Recommendations(IManifest manifest)
            : base(manifest)
        {
        }

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
