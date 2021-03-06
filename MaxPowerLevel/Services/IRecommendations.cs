using System.Collections.Generic;
using System.Threading.Tasks;
using Destiny2;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services
{
    public interface IRecommendations
    {
        Task<IEnumerable<Recommendation>> GetRecommendations(CharacterRecomendationInfo info);
        Task<IDictionary<long, IEnumerable<Recommendation>>> GetRecommendations(IDictionary<long, CharacterRecomendationInfo> infos);
        IEnumerable<Engram> GetEngramPowerLevels(decimal powerLevel);
        Task<SeasonPassInfo> GetSeasonPassInfo(IDictionary<uint, DestinyProgression> progression);
    }
}