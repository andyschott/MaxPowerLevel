using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Destiny2;
using Destiny2.Entities.Items;
using MaxPowerLevel.Models;
using MaxPowerLevel.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MaxPowerLevel.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class CharacterController : Controller
    {
        private readonly IDestiny _destiny;
        private readonly IMaxPowerService _maxPower;
        private readonly IHttpContextAccessor _contextAccessor;

        public CharacterController(IDestiny destiny, IMaxPowerService maxPower,
            IHttpContextAccessor contextAccessor)
        {
            _destiny = destiny;
            _maxPower = maxPower;
            _contextAccessor = contextAccessor;
        }
        
        [HttpGet("{type}/{id}/{characterId}")]
        public async Task<IActionResult> Details(int type, long id, long characterId)
        {
            var accessToken = await _contextAccessor.HttpContext.GetTokenAsync("access_token");

            var membershipType = (BungieMembershipType)type;

            var maxGear = await _maxPower.ComputeMaxPowerAsync(membershipType, id, characterId);
            if(maxGear == null)
            {
                var url = Url.RouteUrl("AccountIndex");
                return Redirect(url);
            }
            var character = await _destiny.GetCharacterInfo(accessToken, membershipType, id, characterId,
                DestinyComponentType.Characters);

            var model = new CharacterViewModel()
            {
                Type = membershipType,
                AccountId = id,
                Items = maxGear.Values,
                MaxPower = _maxPower.ComputePower(maxGear.Values),
                EmblemPath = "https://www.bungie.net/" + character.Character.Data.EmblemPath,
                EmblemBackgroundPath = "htps://www.bungie.net/" + character.Character.Data.EmblemBackgroundPath
            };

            return View(model);
        }
    }
}