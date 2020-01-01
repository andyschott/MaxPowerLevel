using System.Collections.Generic;
using System.Threading.Tasks;
using Destiny2;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services
{
    public interface IRecommendations
    {
        Task<IEnumerable<Recommendation>> GetRecommendations(IEnumerable<Item> items,
            IEnumerable<Item> lowestItems, decimal powerLevel,
            IDictionary<uint, DestinyProgression> progression);
        IEnumerable<Engram> GetEngramPowerLevels(decimal powerLevel);
    }
}