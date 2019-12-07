using System;
using System.Collections.Generic;
using System.Linq;
using Destiny2;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Helpers
{
    public static class MaxPower
    {
        public static decimal ComputePower(IEnumerable<Item> items)
        {
            var power = items.Select(item => item.PowerLevel)
                .Select(powerLevel => Convert.ToDecimal(powerLevel))
                .Average();
            return power;
        }
        public static IEnumerable<Item> FindMax(params IEnumerable<Item>[] items)
        {
            if(!items.Any())
            {
                return Enumerable.Empty<Item>();
            }

            // The first item can just be added to the list.
            var maxItems = new List<Item>();
            maxItems.Add(items[0].First());
            int? exoticIndex = null;
            if(maxItems[0].Tier == TierType.Exotic)
            {
                exoticIndex = 0;
            }

            for(var index = 1; index < items.Length; ++index)
            {
                var maxItem = items[index].First();
                if(maxItem.Tier != TierType.Exotic)
                {
                    // This item isn't exotic. Add it to the list and move on.
                    maxItems.Add(maxItem);
                    continue;
                }

                // If a previous item is an exotic, determine which slot
                // has the higher non-exotic.
                if(exoticIndex == null)
                {
                    // No other exotics. Use this one and move on to the
                    // next slot.
                    maxItems.Add(maxItem);
                    exoticIndex = index;
                    continue;
                }
 
                var prevExotic = maxItems[exoticIndex.Value];
                if(prevExotic.PowerLevel == maxItem.PowerLevel)
                {
                    var prevLegendaryMaxItem = items[exoticIndex.Value]
                        .FirstOrDefault(item => item.Tier != TierType.Exotic);
                    if(null == prevLegendaryMaxItem)
                    {
                        // Only exotics in the previous slot. Use a non-exotic
                        // in this slot.
                        maxItem = items[index].First(item => item.Tier != TierType.Exotic);
                        maxItems.Add(maxItem);
                        continue;
                    }

                    var curLegendaryMaxItem = items[index]
                        .FirstOrDefault(item => item.Tier != TierType.Exotic);
                    if(null == curLegendaryMaxItem)
                    {
                        // Only exotics in the current slot. Use a non-exotic
                        // in the previous slot.
                        var prevMaxItem = items[exoticIndex.Value]
                            .First(item => item.Tier != TierType.Exotic);
                        maxItems[exoticIndex.Value] = prevMaxItem;

                        maxItems.Add(maxItem);
                        exoticIndex = index;
                        continue;
                    }

                    if(prevLegendaryMaxItem.PowerLevel > curLegendaryMaxItem.PowerLevel)
                    {
                        // Use the exotic in this slot instead of the previous one.
                        maxItems[exoticIndex.Value] = prevLegendaryMaxItem;
                        maxItems.Add(maxItem);
                        exoticIndex = index;
                        continue;
                    }
                    else
                    {
                        // Use the exotic in the previous slot.
                        maxItems.Add(curLegendaryMaxItem);
                        continue;
                    }
                }
                else if(prevExotic.PowerLevel > maxItem.PowerLevel)
                {
                    // Use the exotic in the previous slot.
                    var maxLegendary = items[index]
                        .FirstOrDefault(item => item.Tier != TierType.Exotic);
                    if(null == maxLegendary)
                    {
                        // No non-exotics in this slot. Use a legendary in the
                        // previous slot.
                        var prevMaxItem = items[exoticIndex.Value]
                            .First(item => item.Tier != TierType.Exotic);
                        maxItems[exoticIndex.Value] = prevMaxItem;

                        maxItems.Add(maxItem);
                        exoticIndex = index;
                        continue;
                    }
                    else
                    {
                        maxItems.Add(maxLegendary);
                        continue;
                    }
                }
                else // prevExotic.PowerLevel < maxItem.powerLevel
                {
                    // use the exotic in this slot.
                    var prevMaxLegendary = items[exoticIndex.Value]
                        .FirstOrDefault(item => item.Tier != TierType.Exotic);
                    if(null == prevMaxLegendary)
                    {
                        // No non-exitcs in the previous slot. Use a 
                        // legendary in this slot.
                        maxItem = items[index].First(item => item.Tier != TierType.Exotic);
                        maxItems.Add(maxItem);
                        continue;
                    }
                    else
                    {
                        maxItems[exoticIndex.Value] = prevMaxLegendary;

                        maxItems.Add(maxItem);
                        exoticIndex = index;
                        continue;
                    }
                }
            }

             return maxItems;
        }
    }
}