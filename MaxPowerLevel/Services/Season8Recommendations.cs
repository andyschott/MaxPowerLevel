using System.Collections.Generic;
using System.Linq;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services
{
    public class Season8Recommendations : IRecommendations
    {
        private const int SoftCap = 900;
        private const int PowerfulCap = 950;
        private const int HardCap = 960;

        public IEnumerable<string> GetRecommendations(IEnumerable<Item> allItems, int powerLevel)
        {
            if(powerLevel < SoftCap)
            {
                return new[]
                {
                    $"Legendary Engrams to increase your power level to {SoftCap}"
                };
            }

            if(powerLevel < PowerfulCap)
            {
                // Recommmend legendary engrams for any slots that could easily be upgraded
                var recommendations = allItems.Where(item => item.PowerLevel <= powerLevel - 2)
                    .OrderBy(item => item.PowerLevel)
                    .GroupBy(item => item.PowerLevel)
                    .Select(items => 
                    {
                        var slotNames = items.Select(item => item.Slot.Name)
                            .OrderBy(slotName => slotName);
                        return $"Legendary Engrams: {string.Join(", ", slotNames)}";
                    })
                    .Concat(new[] { "Powerful Engrams"})
                    .Concat(new[] { "Pinnacle Engrams" });
                return recommendations;
            }

            if(powerLevel < HardCap)
            {
                return new[] { "Pinnacle Engrams" };
            }

            // At the hard cap. Nothing to do.
            return Enumerable.Empty<string>();
        }
    }
}