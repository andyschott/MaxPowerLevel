using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Destiny2;
using MaxPowerLevel.Helpers;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services
{
    public abstract class AbstractRecommendations : IRecommendations
    {
        protected abstract int SoftCap { get; }
        protected abstract int PowerfulCap { get; }
        protected abstract int HardCap { get; }
        protected abstract uint SeasonHash { get; }
        protected abstract int TargetRankPlus20Power { get; }
        protected virtual DateTime? EndDateOverride { get; } = null;

        // Items purchased from Vendors are 20 power levels below the character's max
        protected virtual int VendorPowerLevelDifference { get; } = 20;

        protected readonly IManifest _manifest;
        private readonly SeasonPass _seasonPass;
        private const int TrailingPowerLevelDifference = 2;

        protected AbstractRecommendations(IManifest manifest, SeasonPass seasonPass)
        {
            _manifest = manifest;
            _seasonPass = seasonPass;
        }

        public async Task<IEnumerable<Recommendation>> GetRecommendations(CharacterRecomendationInfo info)
        {
            var seasonPassSlots = await _seasonPass.LoadAvailableSeasonPassItems(SeasonHash, info.Progressions);
            return GetRecommendations(info, seasonPassSlots);
        }

        public async Task<IDictionary<long, IEnumerable<Recommendation>>> GetRecommendations(IDictionary<long, CharacterRecomendationInfo> infos)
        {
            var seasonPassSlots = await GetSeasonPassSlots(infos);

            var results = infos.Select(info =>
            {
                var characterSeasonPassRewords = seasonPassSlots[info.Key];
                return (info.Key, GetRecommendations(info.Value, characterSeasonPassRewords));
            });

            return results.ToDictionary(item => item.Key, item => item.Item2);
        }

        private IEnumerable<Recommendation> GetRecommendations(CharacterRecomendationInfo info,
            IDictionary<ItemSlot.SlotHashes, int> seasonPassSlots)
        {
            if(info.IntPowerLevel < SoftCap)
            {
                var collections = GetVendorRecommendations(info.Items, info.IntPowerLevel);
                return collections.Concat(new[]
                {
                    new Recommendation($"Rare and Legendary Engrams to increase your power level to {SoftCap}")
                });
            }

            if(info.IntPowerLevel < PowerfulCap)
            {
                var recommendations = new List<Recommendation>(GetVendorRecommendations(info.Items, info.IntPowerLevel));

                // Recommmend legendary engrams for any slots that could easily be upgraded
                recommendations.AddRange(CombineItems(info.Items, info.IntPowerLevel - 2, "Rare/Legendary Engrams"));

                var seasonPassRewards = GetItemRecommendations(info.Items, seasonPassSlots, info.IntPowerLevel, 1);
                if(seasonPassRewards.Any())
                {
                    var recommendation = new Recommendation(GetDisplayString("Season Pass Rewards", seasonPassRewards));
                    recommendations.Add(recommendation);
                }

                recommendations.Add(new Recommendation("Powerful Engrams"));
                recommendations.Add(new Recommendation("Pinnacle Engrams"));

                return  recommendations;
            }

            if(info.IntPowerLevel < HardCap)
            {
                var recommendations = new List<Recommendation>();

                var seasonPassRewards = GetSeasonPassPinnacleRecomendations(info.Items, seasonPassSlots, info.PowerLevel);
                if (seasonPassRewards.Any())
                {
                    var recommendation = new Recommendation(GetDisplayString("Season Pass Rewards", seasonPassRewards));
                    recommendations.Add(recommendation);
                }

                // If any slot is at least two power levels behind,
                // a Powerful Engram would increase the max power level.
                // Ignore any slots where a season pass reward can be used first.
                var trailingSlots = info.Items.Where(item => info.IntPowerLevel - item.PowerLevel >= TrailingPowerLevelDifference)
                    .OrderBy(item => item.PowerLevel)
                    .Select(item => (item.Slot, 1))
                    .Except(seasonPassRewards, new ItemComparer());
                if(trailingSlots.Any())
                {
                    var recommendation = new Recommendation(GetDisplayString("Powerful Engrams", trailingSlots));
                    recommendations.Add(recommendation);
                }

                recommendations.AddRange(CreatePinnacleRecommendations(info.IntPowerLevel, info.Items));
                return recommendations;
            }

            // At the hard cap. Nothing to do.
            return Enumerable.Empty<Recommendation>();
        }

        public IEnumerable<Engram> GetEngramPowerLevels(decimal powerLevel)
        {
            var intPowerLevel = (int)Math.Floor(powerLevel);

            if (intPowerLevel < SoftCap)
            {
                return new[]
                {
                    new Engram("Rare/Legendary Engram",  intPowerLevel + 4, intPowerLevel + 6),
                    new Engram("Prime Engram", intPowerLevel + 7)
                };
            }

            if (powerLevel < PowerfulCap)
            {
                return new[]
                {
                    new Engram("Rare and Legendary Engram", intPowerLevel - 3, Math.Min(intPowerLevel, PowerfulCap)),
                    new Engram("Prime Engram", Math.Max(intPowerLevel + 3, PowerfulCap)),
                    new Engram("Powerful Engram (Tier 1)", Math.Min(intPowerLevel + 3, PowerfulCap)),
                    new Engram("Powerful Engram (Tier 2)", Math.Min(intPowerLevel + 5, PowerfulCap)),
                    new Engram("Powerful Engram (Tier 3)", Math.Min(intPowerLevel + 6, PowerfulCap)),
                    new Engram("Pinnacle Engram (Weak)", Math.Min(intPowerLevel + 4, PowerfulCap + 1)),
                    new Engram("Pinnacle Engram", Math.Min(intPowerLevel + 5, PowerfulCap + 2))
                };
            }

            if (powerLevel <= HardCap)
            {
                return new[]
                {
                    new Engram("Rare/Legendary Engram", Math.Min(intPowerLevel - 3, PowerfulCap), PowerfulCap),
                    new Engram("Prime Engram", intPowerLevel),
                    new Engram("Powerful Engram", intPowerLevel),
                    new Engram("Season Pass Items", intPowerLevel),
                    new Engram("Pinnacle Engram (Weak)", Math.Min(intPowerLevel + 1, HardCap)),
                    new Engram("Pinnacle Engram", Math.Min(intPowerLevel + 2, HardCap))
                };
            }

            throw new Exception($"Unknown power level {intPowerLevel}");
        }

        public async Task<SeasonPassInfo> GetSeasonPassInfo(IDictionary<uint, DestinyProgression> progression)
        {
            var season = await _manifest.LoadSeason(SeasonHash);
            var seasonPass = await _manifest.LoadSeasonPass(season.SeasonPassHash);

            if(season.SeasonPassProgressionHash == 0)
            {
                return null;
            }

            var baseProgression = progression[season.SeasonPassProgressionHash];
            var prestigeProgression = progression[seasonPass.PrestigeProgressionHash];

            var rank = baseProgression.Level + prestigeProgression.Level;

            var seasonEndDate = EndDateOverride ?? season.EndDate.Value;
            return new SeasonPassInfo(season.DisplayProperties.Name, seasonEndDate, rank, TargetRankPlus20Power);
        }

        private Task<IDictionary<long, IDictionary<ItemSlot.SlotHashes, int>>> GetSeasonPassSlots(IDictionary<long, CharacterRecomendationInfo> infos)
        {
            var progressions = infos.ToDictionary(item => item.Key, item => item.Value.Progressions);
            return  _seasonPass.LoadAvailableSeasonPassItems(SeasonHash, progressions);
        }

        private IEnumerable<Recommendation> GetVendorRecommendations(IEnumerable<Item> allItems, int powerLevel)
        {
            return CombineItems(allItems, powerLevel - VendorPowerLevelDifference,
                "Vendor Engrams");
        }

        private static IEnumerable<Recommendation> CombineItems(IEnumerable<Item> allItems,
            int powerLevel, string description)
        {
            return allItems.Where(item => item.PowerLevel <= powerLevel)
                .OrderBy(item => item.PowerLevel)
                .GroupBy(item => item.PowerLevel)
                .Select(items =>
                {
                    var slotNames = items.Select(item => item.Slot.Name)
                        .OrderBy(slotName => slotName);
                    return new Recommendation($"{description}: {string.Join(", ", slotNames)}");
                });
        }

        private static IEnumerable<(ItemSlot slot, int count)> GetItemRecommendations(IEnumerable<Item> allItems,
            IDictionary<ItemSlot.SlotHashes, int> availableSlots, int powerLevel, int difference)
        {
            var slotUpgrades = allItems.Where(item =>
            {
                if(!availableSlots.ContainsKey(item.Slot.Hash))
                {
                    return false;
                }

                return powerLevel - item.PowerLevel >= difference;
            }).OrderBy(item => item.PowerLevel)
            .Select(item => (item.Slot, availableSlots[item.Slot.Hash]));

            return slotUpgrades;
        }

        private static string GetDisplayString(string description, IEnumerable<(ItemSlot slot, int count)> slots)
        {
            var slotNames = slots.Select(item =>
            {
                if(item.count == 1)
                {
                    return item.slot.Name;
                }

                return $"{item.slot.Name} ({item.count})";
            });
            return $"{description} ({string.Join(", ", slotNames)})";
        }

        protected abstract IEnumerable<PinnacleActivity> CreatePinnacleActivities();
        protected virtual IEnumerable<PinnacleActivity> CreateWeakPinnacleActivities() => Enumerable.Empty<PinnacleActivity>();

        protected virtual IEnumerable<Recommendation> CreatePinnacleRecommendations(int powerLevel, IEnumerable<Item> items)
        {
            var powerLevels = items.ToDictionary(item => item.Slot.Hash, item => (decimal)item.PowerLevel);

            var strongPinnacles = new Recommendation("Pinnacle Engrams",
                SortPinnacleActivites(CreatePinnacleActivities(), powerLevels));
            var weakPinnacles = new Recommendation("Pinnacle Engrams (Weak)",
                SortPinnacleActivites(CreateWeakPinnacleActivities(), powerLevels));

            var levelsToGo = HardCap - powerLevel;
            if (levelsToGo <= 2)
            {
                // If within 1 of the hard cap, order doesn't matter.
                if(levelsToGo <= 1)
                {
                    return new[]
                    {
                        new Recommendation("Pinnacle Engrams",
                            strongPinnacles.Activities.Concat(weakPinnacles.Activities)
                                .OrderBy(activity => activity.FirstOrDefault()))
                    };
                }

                // If within 2 of the hard cap, do normal pinnacles first.
                return new[]
                {
                    strongPinnacles,
                    weakPinnacles,
                };
            }
            
            // If less than half of the slots would get an upgrade from a weak pinnacle,
            // recommend strong pinnacles first.
            var numWeakUpgrades = powerLevels.Values.Count(slotPowerLevel => powerLevel - slotPowerLevel > 1);
            if(numWeakUpgrades < powerLevels.Count / 2)
            {
            return new[]
            {
                strongPinnacles,
                weakPinnacles,
            };
            }

            // Not sure which is better - suggesting weak then strong for now
            return new[]
            {
                weakPinnacles,
                strongPinnacles,
            };
        }

        protected IEnumerable<IEnumerable<string>> SortPinnacleActivites(IEnumerable<PinnacleActivity> pinnacleActivities,
            IDictionary<ItemSlot.SlotHashes, decimal> powerLevels)
        {
            var activitiesWithUpgrades = pinnacleActivities.ToDictionary(activity => activity.Name,
                activity => AveragePowerGain(powerLevels, activity));

            var prioritizedActivities = activitiesWithUpgrades.GroupBy(activity => activity.Value, activity => activity.Key)
                .Where(group => group.Key > 0)
                .OrderByDescending(group => group.Key)
                .Select(group => group.OrderBy(activity => activity));
            
            return prioritizedActivities;
        }

        private decimal AveragePowerGain(IDictionary<ItemSlot.SlotHashes, decimal> powerLevels,
             PinnacleActivity activity)
        {
            // 1. Find all combination of drops across all encounters in the activity
            var itr = new EncounterIterator(activity);
            
            // 2. Compute the power gain for each combination of encounters
            var possiblePowerGains = itr.Select(combo =>
            {
                var currentPowerLevels = new Dictionary<ItemSlot.SlotHashes, decimal>(powerLevels);
                var startingPowerLevel = currentPowerLevels.Values.Average();

                foreach(var slot in combo)
                {
                    var pinnacleItemLevel = Math.Min(Math.Floor(currentPowerLevels.Values.Average()) + 2, HardCap);
                    currentPowerLevels[slot] = Math.Max(pinnacleItemLevel, currentPowerLevels[slot]);
                }

                var finalPowerLevel = currentPowerLevels.Values.Average();
                return finalPowerLevel - startingPowerLevel;
            });

            // 3. Return the average of all possible power gains
            return possiblePowerGains.Average();
        }

        private static IEnumerable<(ItemSlot slot, int count)> GetSeasonPassPinnacleRecomendations(IEnumerable<Item> allItems,
            IDictionary<ItemSlot.SlotHashes, int> seasonPassRewards, decimal powerLevel)
        {
            // Find all of the slots where season pass items are a higher power level
            var seasonPassUpgrades = allItems.Where(item => seasonPassRewards.ContainsKey(item.Slot.Hash))
                .Select(item => (item, powerDifference: (int)powerLevel - item.PowerLevel))
                .Where(item => item.powerDifference > 0)
                .OrderByDescending(item => item.powerDifference)
                .Select(item => (slot: item.item.Slot, count: seasonPassRewards[item.item.Slot.Hash]));

            decimal computePowerLevel(ICollection<int> powerLevels)
            {
                return powerLevels.Sum() / powerLevels.Count;
            }

            var potentialItems = allItems.ToDictionary(item => item.Slot.Hash, item => item.PowerLevel);
            var slotUpgrades = new List<(ItemSlot, int)>();
            foreach(var seasonPassUpgrade in seasonPassUpgrades)
            {
                var potentialPowerLevel = computePowerLevel(potentialItems.Values);
                if (potentialPowerLevel - (int)powerLevel >= 1)
                {
                    break;
                }

                potentialItems[seasonPassUpgrade.slot.Hash] = (int)powerLevel;
                slotUpgrades.Add(seasonPassUpgrade);
            }

            // If season pass items won't increase power level, don't reommend anything.
            if(computePowerLevel(potentialItems.Values) - (int)powerLevel < 1)
            {
                return Enumerable.Empty<(ItemSlot, int)>();
            }

            return slotUpgrades;
        }

        class ItemComparer : IEqualityComparer<(ItemSlot slot, int count)>
        {
            public bool Equals([AllowNull] (ItemSlot slot, int count) x, [AllowNull] (ItemSlot slot, int count) y)
            {
                return x.slot.Hash == y.slot.Hash;
            }

            public int GetHashCode([DisallowNull] (ItemSlot slot, int count) obj)
            {
                return (int)obj.slot.Hash;
            }
        }
    }
}
