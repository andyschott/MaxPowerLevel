using System.Collections.Generic;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services
{
    public interface IRecommendations
    {
        IEnumerable<string> GetRecommendations(IEnumerable<Item> items, int powerLevel);
        IEnumerable<Engram> GetEngramPowerLevels(int powerLevel);
    }
}