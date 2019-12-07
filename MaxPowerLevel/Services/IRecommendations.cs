using System.Collections.Generic;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services
{
    public interface IRecommendations
    {
        IEnumerable<string> GetRecommendations(IEnumerable<Item> items, decimal powerLevel);
        IEnumerable<Engram> GetEngramPowerLevels(decimal powerLevel);
    }
}