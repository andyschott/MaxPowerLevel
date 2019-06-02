using System.Threading.Tasks;
using Destiny2;
using Destiny2.Responses;
using Destiny2.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace MaxPowerLevel.Services
{
  public class DestinyService : IDestinyService
  {
      private IConfiguration _config;
      private IHttpContextAccessor _contextAccessor;

      public DestinyService(IConfiguration config, IHttpContextAccessor contextAccessor)
      {
          _config = config;
          _contextAccessor = contextAccessor;
      }

      public async Task<UserMembershipData> GetMembershipDataAsync(long membershipId)
      {
          using(var destiny = await CreateDestinyAsync())
          {
              return await destiny.GetMembershipData(membershipId);
          }
      }
      
      public async Task<DestinyProfileResponse> GetProfileAsync(BungieMembershipType type, long id)
      {
          using(var destiny = await CreateDestinyAsync())
          {
              return await destiny.GetProfile(type, id);
          }
      }
      
      public async Task<DestinyProfileResponse> GetProfileAsync(BungieMembershipType type, long id, params DestinyComponentType[] components)
      {
          using(var destiny = await CreateDestinyAsync())
          {
              return await destiny.GetProfile(type, id, components);
          }
      }

      public async Task<DestinyCharacterResponse> GetCharacterInfoAsync(BungieMembershipType type, long id, long characterId,
                                                                        params DestinyComponentType[] infos)
      {
          using(var destiny = await CreateDestinyAsync())
          {
              return await destiny.GetCharacterInfo(type, id, characterId, infos);
          }
      }

      public async Task<DestinyItemResponse> GetItemAsync(BungieMembershipType type, long id, long itemInstanceId,
                                                          params DestinyComponentType[] infos)
      {
          using(var destiny = await CreateDestinyAsync())
          {
              return await destiny.GetItem(type, id, itemInstanceId, infos);
          }
      }

      private async Task<Destiny> CreateDestinyAsync()
      {
          var accessToken = await _contextAccessor.HttpContext.GetTokenAsync("access_token");
          return new Destiny(_config["Bungie:ApiKey"], accessToken);
      }
  }
}