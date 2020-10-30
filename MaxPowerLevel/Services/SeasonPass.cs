using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Destiny2;
using Destiny2.Definitions;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services
{
    public class SeasonPass
    {
        private static readonly ISet<ItemSlot.SlotHashes> _slotHashes =
            new HashSet<ItemSlot.SlotHashes>((ItemSlot.SlotHashes[])Enum.GetValues(typeof(ItemSlot.SlotHashes)));
        private readonly IManifest _manifest;

        public SeasonPass(IManifest manifest)
        {
            _manifest = manifest;
        }

        public async Task<IDictionary<ItemSlot.SlotHashes, int>> LoadAvailableSeasonPassItems(uint seasonHash, IDictionary<uint, DestinyProgression> progression)
        {
            var (seasonPassProgressionHash, progressionDefinition) = await GetSeasonPassProgress(seasonHash);
            if(seasonPassProgressionHash == 0)
            {
                return new Dictionary<ItemSlot.SlotHashes, int>();
            }
            
            return await LoadAvailableSeasonPassItems(progression[seasonPassProgressionHash], progressionDefinition);
        }

        public async Task<IDictionary<long, IDictionary<ItemSlot.SlotHashes, int>>> LoadAvailableSeasonPassItems(uint seasonHash,
            IDictionary<long, IDictionary<uint, DestinyProgression>> progressions)
        {
            var (seasonPassProgressionHash, progressionDefinition) = await GetSeasonPassProgress(seasonHash);
            if(seasonPassProgressionHash == 0)
            {
                return new Dictionary<long, IDictionary<ItemSlot.SlotHashes, int>>();
            }
            
            var availableItemTasks = progressions.Select(async progression =>
            {
                var seasonPassProgression = progression.Value[seasonPassProgressionHash];
                var availableItems = await LoadAvailableSeasonPassItems(seasonPassProgression, progressionDefinition);

                return (progression.Key, availableItems);
            });

            var availableItems = await Task.WhenAll(availableItemTasks);

            return availableItems.ToDictionary(item => item.Key, item => item.availableItems);
        }

        private async Task<(uint seasonPassProgressionHash, DestinyProgressionDefinition progressionDefinition)> GetSeasonPassProgress(uint seasonHash)
        {
            var season = await _manifest.LoadSeason(seasonHash);
            if(season.SeasonPassProgressionHash == 0)
            {
                return (0, null);
            }

            var progressionDefinition = await _manifest.LoadProgression(season.SeasonPassProgressionHash);
            return (season.SeasonPassProgressionHash, progressionDefinition);
        }

        private async Task<IDictionary<ItemSlot.SlotHashes, int>> LoadAvailableSeasonPassItems(DestinyProgression seasonPassProgression,
            DestinyProgressionDefinition seasonPassProgressionDef)
        {
            if(seasonPassProgression.RewardItemStates == null)
            {
                return new Dictionary<ItemSlot.SlotHashes, int>();
            }
            
            var seasonPassRewards = seasonPassProgression.RewardItemStates.ToArray();

            // Find all of the rewards that are available but unclaimed
            var availableRewards = seasonPassProgressionDef.RewardItems.Where((rewardItem, index) =>
            {
                var state = seasonPassRewards[index];
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
    }
}