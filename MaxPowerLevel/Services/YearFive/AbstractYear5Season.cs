using System.Collections.Generic;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services.YearFive
{
    public abstract class AbstractYear5Season : AbstractSeason
    {
        protected static readonly PinnacleActivity _graspOfAvarice = new PinnacleActivity("Grasp of Avarice", new[] { AllSlots });

        protected override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            return new[]
            {
                NightfallScore,
                _graspOfAvarice
            };
        }
    }
}