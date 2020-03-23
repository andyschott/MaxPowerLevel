using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Destiny2;
using Destiny2.Definitions;
using MaxPowerLevel.Helpers;
using MaxPowerLevel.Models;
using VendorEngrams;

namespace MaxPowerLevel.Services
{
    public abstract class AbstractRecommendations : IRecommendations
    {
        protected abstract int SoftCap { get; }
        protected abstract int PowerfulCap { get; }
        protected abstract int HardCap { get; }
        protected abstract uint SeasonHash { get; }
        protected abstract int TargetRankPlus20Power { get; }

        // Items pulled from Collections are 20 power levels below the character's max
        protected virtual int CollectionsPowerLevelDifference { get; }= 20;

        protected readonly IManifest _manifest;
        protected readonly IVendorEngramsClient _vendorEngrams;

        protected static readonly ISet<ItemSlot.SlotHashes> _slotHashes =
            new HashSet<ItemSlot.SlotHashes>((ItemSlot.SlotHashes[])Enum.GetValues(typeof(ItemSlot.SlotHashes)));

        private const int TrailingPowerLevelDifference = 2;

        protected AbstractRecommendations(IManifest manifest, IVendorEngramsClient vendorEngrams)
        {
            _manifest = manifest;
            _vendorEngrams = vendorEngrams;
        }

        public async Task<IEnumerable<Recommendation>> GetRecommendations(IEnumerable<Item> allItems,
            decimal powerLevel, IDictionary<uint, DestinyProgression> progressions)
        {
            var intPowerLevel = (int)Math.Floor(powerLevel);

            var collections = GetCollectionsRecommendations(allItems, intPowerLevel);

            if(intPowerLevel < SoftCap)
            {
                return collections.Concat(new[]
                {
                    new Recommendation($"Rare, Legendary, and Vendor Engrams to increase your power level to {SoftCap}")
                });
            }

            var seasonPassSlots = await LoadAvailableSeasonPassItems(progressions);
            var seasonPassRewards = GetItemRecommendations(allItems, seasonPassSlots, intPowerLevel, TrailingPowerLevelDifference);

            if(intPowerLevel < PowerfulCap)
            {
                // Recommmend legendary engrams for any slots that could easily be upgraded
                var legendary = CombineItems(allItems, intPowerLevel - 2, "Rare/Legendary Engrams");
                var recommendations = new List<Recommendation>(legendary);
                var vendors = await CreateVendorRecommendations(allItems, intPowerLevel);
                if(vendors.Any())
                {
                    recommendations.AddRange(vendors);
                }

                if(seasonPassRewards.Any())
                {
                    var recommendation = new Recommendation(GetDisplayString("Season Pass Rewards", seasonPassRewards));
                    recommendations.Add(recommendation);
                }

                recommendations.Add(new Recommendation("Powerful Engrams"));
                recommendations.Add(new Recommendation("Pinnacle Engrams"));

                return  recommendations;
            }

            if(intPowerLevel < HardCap)
            {
                var recommendations = new List<Recommendation>();

                if(seasonPassRewards.Any())
                {
                    var recommendation = new Recommendation(GetDisplayString("Season Pass Rewards", seasonPassRewards));
                    recommendations.Add(recommendation);
                }

                // If any slot is at least two power levels behind,
                // a Powerful Engram would increase the max power level.
                // Ignore any slots where a season pass reward can be used first.
                var trailingSlots = allItems.Where(item => intPowerLevel - item.PowerLevel >= TrailingPowerLevelDifference)
                    .OrderBy(item => item.PowerLevel)
                    .Select(item => (item.Slot, 1))
                    .Except(seasonPassRewards, new ItemComparer());
                if(trailingSlots.Any())
                {
                    var recommendation = new Recommendation(GetDisplayString("Powerful Engrams", trailingSlots));
                    recommendations.Add(recommendation);
                }

                recommendations.Add(CreatePinnacleRecommendations(intPowerLevel, allItems));
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
                    // TODO: Verify power level of engrams before the soft cap
                    new Engram("Rare/Legendary Engram",  intPowerLevel + 1, intPowerLevel + 2)
                };
            }

            if (powerLevel < PowerfulCap)
            {
                return new[]
                {
                    new Engram("Rare, Legendary, and Vendor Engram", intPowerLevel - 3, Math.Min(intPowerLevel, PowerfulCap)),
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
                    new Engram("Rare/Legendary Engram", Math.Min(intPowerLevel - 3, PowerfulCap), PowerfulCap),
                    new Engram("Powerful Engram (Tier 1)", intPowerLevel),
                    new Engram("Powerful Engram (Tier 2)", intPowerLevel),
                    new Engram("Powerful Engram (Tier 3)", intPowerLevel),
                    new Engram("Season Pass Items", intPowerLevel),
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

            return new SeasonPassInfo(season.DisplayProperties.Name, season.EndDate.Value, rank, TargetRankPlus20Power);
        }

        private IEnumerable<Recommendation> GetCollectionsRecommendations(IEnumerable<Item> allItems, int powerLevel)
        {
            return CombineItems(allItems, powerLevel - CollectionsPowerLevelDifference,
                "Pull from Collections");
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

        protected async Task<IDictionary<ItemSlot.SlotHashes, int>> LoadAvailableSeasonPassItems(IDictionary<uint, DestinyProgression> progression)
        {
            var season = await _manifest.LoadSeason(SeasonHash);
            if(season.SeasonPassProgressionHash == 0)
            {
                return new Dictionary<ItemSlot.SlotHashes, int>();
            }
            
            var progressionDefinition = await _manifest.LoadProgression(season.SeasonPassProgressionHash);

            var characterProgression = progression[season.SeasonPassProgressionHash];

            var characterRewards = characterProgression.RewardItemStates.ToArray();

            // Find all of the rewards that are available but unclaimed
            var availableRewards = progressionDefinition.RewardItems.Where((rewardItem, index) =>
            {
                var state = characterRewards[index];
                if(state.HasFlag(DestinyProgressionRewardItemState.Invisible))
                {
                    return false;
                }

                if(state.HasFlag(DestinyProgressionRewardItemState.Claimed))
                {
                    return false;
                }

                return state.HasFlag(DestinyProgressionRewardItemState.Earned | DestinyProgressionRewardItemState.ClaimAllowed);
            });

            var availableSlots = new Dictionary<ItemSlot.SlotHashes, int>();
            foreach(var reward in availableRewards)
            {
                var itemDef = await _manifest.LoadInventoryItem(reward.ItemHash);
                var slotHash = (ItemSlot.SlotHashes)itemDef.Inventory.BucketTypeHash;
                if (itemDef == null || !_slotHashes.Contains(slotHash))
                {
                    continue;
                }

                availableSlots.TryGetValue(slotHash, out var count);
                availableSlots[slotHash] = count + 1;
            }

            return availableSlots;
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

        protected virtual Recommendation CreatePinnacleRecommendations(int powerLevel, IEnumerable<Item> items)
        {
            var pinnacleActivities = CreatePinnacleActivities();

            var powerLevels = items.ToDictionary(item => item.Slot.Hash, item => (decimal)item.PowerLevel);

            var activitiesWithUpgrades = pinnacleActivities.ToDictionary(activity => activity.Name,
                activity => AveragePowerGain(powerLevels, activity));

            var prioritizedActivities = activitiesWithUpgrades.GroupBy(activity => activity.Value, activity => activity.Key)
                .Where(group => group.Key > 0)
                .OrderByDescending(group => group.Key)
                .Select(group => string.Join(" / ", group.OrderBy(activity => activity)));

            return new Recommendation("Pinnacle Engrams", prioritizedActivities);
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

        private async Task<IEnumerable<Recommendation>> CreateVendorRecommendations(IEnumerable<Item> items, int characterPowerLevel)
        {
            if(characterPowerLevel >= PowerfulCap)
            {
                // Nothing to recommend
                return Enumerable.Empty<Recommendation>();
            }

            var vendorEngrams = await _vendorEngrams.GetVendorDrops();
            var lookup = vendorEngrams.ToLookup(vendorEngram => vendorEngram.Drop);

            var vendorTasks = vendorEngrams.Select(vendor => _manifest.LoadVendor(vendor.Hash));
            var vendors = (await Task.WhenAll(vendorTasks)).ToDictionary(vendor => vendor.Hash);

            var lowRecommendation = CreateVendorRecommendation(vendors, lookup[DropStatus.Low],
                items, characterPowerLevel - 3, characterPowerLevel, "Vendor Engrams - Low");
            var highRecommendation = CreateVendorRecommendation(vendors, lookup[DropStatus.High],
                items, characterPowerLevel, characterPowerLevel, "Vendor Engrams - High");

            return new[] { lowRecommendation, highRecommendation }.Where(recommendation => recommendation != null);
        }

        private Recommendation CreateVendorRecommendation(IDictionary<uint, DestinyVendorDefinition> vendors,
            IEnumerable<Vendor> engrams, IEnumerable<Item> items, int engramPowerLevel, int powerLevel,
            string description)
        {
            var recommendedVendors = engrams.Select(vendor =>
            {
                var slots = VendorEngramSlots.GetEngramSlots(vendor.Hash)
                    .ToDictionary(slotHash => slotHash, slotHash => engramPowerLevel);
                var itemRecommendations = GetItemRecommendations(items, slots, engramPowerLevel, 1);
                if(!itemRecommendations.Any())
                {
                    return null;
                }

                var vendorName = vendors[vendor.Hash].DisplayProperties.Name;
                return GetDisplayString(vendorName, itemRecommendations);
            }).Where(thing => thing != null).OrderBy(thing => thing);

            if(!recommendedVendors.Any())
            {
                return null;
            }

            return new Recommendation(description, recommendedVendors);
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
