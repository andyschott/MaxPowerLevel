using System.Collections.Generic;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services.YearFive
{
    public abstract class AbstractYear5Season : AbstractSeason
    {
        protected static readonly PinnacleActivity _graspOfAvarice = new PinnacleActivity("Grasp of Avarice", new[] { AllSlots });
        protected static readonly PinnacleActivity _daresOfEternity = new PinnacleActivity("Dares of Eternity Pinnacle Challenge", new[] { AllSlots });
        protected static readonly PinnacleActivity _voxObscura = new PinnacleActivity("Vox Obscura", new[] { AllSlots });
        protected static readonly PinnacleActivity _wellspring = new PinnacleActivity("Wellspring Pinnacle Challenge", new[] { AllSlots });

        protected override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            return new[]
            {
                NightfallScore,
                _graspOfAvarice,
                _daresOfEternity,
                _voxObscura,
                _wellspring
            };
        }
    }
}