using System;
using System.Collections.Generic;
using System.Linq;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services
{
    public abstract class AbstractRecommendations : IRecommendations
    {
        protected abstract int SoftCap { get; }
        protected abstract int PowerfulCap { get; }
        protected abstract int HardCap { get; }

        // Items pulled from Collections are 20 power levels below the character's max
        protected virtual int CollectionsPowerLevelDifference { get; }= 20;

        public IEnumerable<string> GetRecommendations(IEnumerable<Item> allItems, decimal powerLevel)
        {
            var intPowerLevel = (int)Math.Floor(powerLevel);

            var collections = GetCollectionsRecommendations(allItems, intPowerLevel);

            if(intPowerLevel < SoftCap)
            {
                return collections.Concat(new[]
                {
                    $"Rare/Legendary Engrams to increase your power level to {SoftCap}"
                });
            }

            if(intPowerLevel < PowerfulCap)
            {
                var legendary = CombineItems(allItems, intPowerLevel - 2, "Rare/Legendary Engrams");
                var powerful = new[] { "Powerful Engrams" };
                var pinnacle = new[] { "Pinnacle Engrams" };

                // Recommend pinnacles once at 947
                if (powerLevel >= 947)
                {
                    return collections.Concat(legendary)
                        .Concat(pinnacle)
                        .Concat(powerful);
                }

                // Recommmend legendary engrams for any slots that could easily be upgraded
                return collections.Concat(CombineItems(allItems, intPowerLevel - 2, "Rare/Legendary Engrams"))
                    .Concat(new[] { "Powerful Engrams" })
                    .Concat(new[] { "Pinnacle Engrams" });
            }

            if(intPowerLevel < HardCap)
            {
                // If any slot is at least two power levels behind,
                // a Powerful Engram would increase the max power level.
                var powerfulEngrams = Enumerable.Empty<string>();

                var trailingSlots = allItems.Where(item => intPowerLevel - item.PowerLevel >= 2);
                if(trailingSlots.Any())
                {
                    var slotNames = trailingSlots.Select(item => item.Slot.Name);
                    powerfulEngrams = new[]
                    {
                        $"Powerful Engrams ({string.Join(", ", slotNames)})"
                    };
                }
                return collections
                    .Concat(powerfulEngrams)
                    .Concat(new[] { "Pinnacle Engrams" });
            }

            // At the hard cap. Nothing to do.
            return Enumerable.Empty<string>();
        }

        public IEnumerable<Engram> GetEngramPowerLevels(decimal powerLevel)
        {
            var intPowerLevel = (int)Math.Floor(powerLevel);

            if (intPowerLevel < SoftCap)
            {
                return new[]
                {
                    // TODO: Verify power level of engrams before the soft cap
                    new Engram("Rare/Legendary Engram",  intPowerLevel + 1, intPowerLevel + 2)
                };
            }

            if (powerLevel < PowerfulCap)
            {
                return new[]
                {
                    new Engram("Rare/Legendary Engram", intPowerLevel - 3, Math.Min(intPowerLevel, PowerfulCap)),
                    new Engram("Powerful Engram (Tier 1)", Math.Min(intPowerLevel + 3, PowerfulCap)),
                    new Engram("Powerful Engram (Tier 2)", Math.Min(intPowerLevel + 5, PowerfulCap)),
                    new Engram("Powerful Engram (Tier 3)", Math.Min(intPowerLevel + 6, PowerfulCap + 1)),
                    new Engram("Pinnacle Engram", Math.Min(intPowerLevel + 4, PowerfulCap + 2), Math.Min(intPowerLevel + 5, PowerfulCap + 2))
                };
            }

            if (powerLevel <= HardCap)
            {
                return new[]
                {
                    new Engram("Rare/Legendary Engram", PowerfulCap - 3, PowerfulCap),
                    new Engram("Powerful Engram (Tier 1)", intPowerLevel),
                    new Engram("Powerful Engram (Tier 2)", intPowerLevel),
                    new Engram("Powerful Engram (Tier 3)", intPowerLevel),
                    new Engram("Pinnacle Engram", intPowerLevel + 2)
                };
            }

            throw new Exception($"Unknown power level {intPowerLevel}");
        }

        private IEnumerable<string> GetCollectionsRecommendations(IEnumerable<Item> allItems, int powerLevel)
        {
            return CombineItems(allItems, powerLevel - CollectionsPowerLevelDifference,
                "Pull from Collections");
        }

        private static IEnumerable<string> CombineItems(IEnumerable<Item> allItems,
            int powerLevel, string description)
        {
            return allItems.Where(item => item.PowerLevel <= powerLevel)
                .OrderBy(item => item.PowerLevel)
                .GroupBy(item => item.PowerLevel)
                .Select(items =>
                {
                    var slotNames = items.Select(item => item.Slot.Name)
                        .OrderBy(slotName => slotName);
                    return $"{description}: {string.Join(", ", slotNames)}";
                });
        }
    }
}
