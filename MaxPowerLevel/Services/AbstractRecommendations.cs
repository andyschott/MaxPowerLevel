using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Destiny2;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services
{
    public abstract class AbstractRecommendations : IRecommendations
    {
        protected abstract int SoftCap { get; }
        protected abstract int PowerfulCap { get; }
        protected abstract int HardCap { get; }
        protected abstract uint SeasonHash { get; }

        // Items pulled from Collections are 20 power levels below the character's max
        protected virtual int CollectionsPowerLevelDifference { get; }= 20;

        protected readonly IManifest _manifest;

        protected static readonly ISet<ItemSlot.SlotHashes> _slotHashes =
            new HashSet<ItemSlot.SlotHashes>((ItemSlot.SlotHashes[])Enum.GetValues(typeof(ItemSlot.SlotHashes)));

        private const int TrailingPowerLevelDifference = 2;

        protected AbstractRecommendations(IManifest manifest)
        {
            _manifest = manifest;
        }

        public async Task<IEnumerable<string>> GetRecommendations(IEnumerable<Item> allItems,
            decimal powerLevel, IDictionary<uint, DestinyProgression> progressions)
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
                // Recommmend legendary engrams for any slots that could easily be upgraded
                var legendary = CombineItems(allItems, intPowerLevel - 2, "Rare/Legendary Engrams");
                var powerful = new[] { "Powerful Engrams" };
                var pinnacle = new[] { "Pinnacle Engrams" };

                return collections.Concat(legendary)
                    .Concat(new[] { "Powerful Engrams" })
                    .Concat(new[] { "Pinnacle Engrams" });
            }

            if(intPowerLevel < HardCap)
            {
                var recommendations = new List<string>();

                var seasonPassSlots = await LoadAvailableSeasonPassItems(progressions);
                var seasonPassRewards = GetSeasonPassRecommendations(allItems, seasonPassSlots, intPowerLevel);
                if(seasonPassRewards.Any())
                {
                    recommendations.Add(GetDisplayString("Season Pass Rewards", seasonPassRewards));
                }

                // If any slot is at least two power levels behind,
                // a Powerful Engram would increase the max power level.
                // Ignore any slots where a season pass reward can be used first.
                var trailingSlots = allItems.Where(item => intPowerLevel - item.PowerLevel >= TrailingPowerLevelDifference)
                    .OrderBy(item => item.PowerLevel)
                    .Select(item => item.Slot)
                    .Except(seasonPassRewards);
                if(trailingSlots.Any())
                {
                    recommendations.Add(GetDisplayString("Powerful Engrams", trailingSlots));
                }

                recommendations.Add("Pinnacle Engrams");
                return recommendations;
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
                    new Engram("Rare/Legendary Engram", Math.Min(intPowerLevel - 3, PowerfulCap), PowerfulCap),
                    new Engram("Powerful Engram (Tier 1)", intPowerLevel),
                    new Engram("Powerful Engram (Tier 2)", intPowerLevel),
                    new Engram("Powerful Engram (Tier 3)", intPowerLevel),
                    new Engram("Pinnacle Engram", Math.Min(intPowerLevel + 2, HardCap))
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

        protected async Task<ISet<ItemSlot.SlotHashes>> LoadAvailableSeasonPassItems(IDictionary<uint, DestinyProgression> progression)
        {
            var season = await _manifest.LoadSeason(SeasonHash);
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

            var availableSlots = new HashSet<ItemSlot.SlotHashes>();
            foreach(var reward in availableRewards)
            {
                var itemDef = await _manifest.LoadInventoryItem(reward.ItemHash);
                var slotHash = (ItemSlot.SlotHashes)itemDef.Inventory.BucketTypeHash;
                if (itemDef == null || !_slotHashes.Contains(slotHash))
                {
                    continue;
                }

                availableSlots.Add(slotHash);
            }

            return availableSlots;
        }

        private static IEnumerable<ItemSlot> GetSeasonPassRecommendations(IEnumerable<Item> allItems,
            ISet<ItemSlot.SlotHashes> avaiableSeasonPassSlots, int powerLevel)
        {
            var slotUpgrades = allItems.Where(item =>
            {
                if(!avaiableSeasonPassSlots.Contains(item.Slot.Hash))
                {
                    return false;
                }

                return powerLevel - item.PowerLevel >= TrailingPowerLevelDifference;
            }).OrderBy(item => item.PowerLevel)
            .Select(item => item.Slot);

            return slotUpgrades;
        }

        private static string GetDisplayString(string description, IEnumerable<ItemSlot> slots)
        {
            var slotNames = slots.Select(slot => slot.Name);
            return $"{description} ({string.Join(", ", slotNames)})";
        }
    }
}
