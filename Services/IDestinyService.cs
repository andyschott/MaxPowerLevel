using System.Threading.Tasks;
using Destiny2;
using Destiny2.Responses;
using Destiny2.User;

namespace MaxPowerLevel.Services
{
    public interface IDestinyService
    {
        Task<UserMembershipData> GetMembershipDataAsync(long membershipId);
        Task<DestinyProfileResponse> GetProfileAsync(BungieMembershipType type, long id);
        Task<DestinyProfileResponse> GetProfileAsync(BungieMembershipType type, long id, params DestinyComponentType[] components);
        Task<DestinyCharacterResponse> GetCharacterInfoAsync(BungieMembershipType type, long id, long characterId, params DestinyComponentType[] infos);
        Task<DestinyItemResponse> GetItemAsync(BungieMembershipType type, long id, long itemInstanceId, params DestinyComponentType[] infos);
    }
}