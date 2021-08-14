using System.Collections.Generic;
using System.Linq;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services.YearFour
{
    public class Season14 : AbstractYear4Season
    {
        public override int SoftCap => 1260;

        public override int PowerfulCap => 1310;

        public override int HardCap => 1320;

        public override uint SeasonHash => 2809059429;

        public override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            return base.CreatePinnacleActivities().Concat(new[]
            {
                _vaultOfGlass,
                _pressage,
                new PinnacleActivity("Corrupted Conflux Chests", new[] { AllSlots }),
            });
        }

        public override IEnumerable<PinnacleActivity> CreateWeakPinnacleActivities()
        {
            return base.CreateWeakPinnacleActivities().Concat(new[]
            {
                new PinnacleActivity("Override Conflux Chests", new[] { AllSlots }),
                new PinnacleActivity("Splicer Servitor Bounties", new[] { AllSlots }),
            });
        }
    }
}