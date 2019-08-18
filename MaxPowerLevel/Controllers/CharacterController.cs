using System.Threading.Tasks;
using Destiny2;
using MaxPowerLevel.Helpers;
using MaxPowerLevel.Models;
using MaxPowerLevel.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MaxPowerLevel.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class CharacterController : Controller
    {
        private readonly IDestiny2 _destiny;
        private readonly IMaxPowerService _maxPower;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOptions<BungieSettings> _bungie;
        private readonly ILogger _logger;

        public CharacterController(IDestiny2 destiny, IMaxPowerService maxPower,
            IHttpContextAccessor contextAccessor, IOptions<BungieSettings> bungie,
            ILogger<CharacterController> logger)
        {
            _destiny = destiny;
            _maxPower = maxPower;
            _contextAccessor = contextAccessor;
            _bungie = bungie;
            _logger = logger;
        }
        
        [HttpGet("{type}/{id}/{characterId}")]
        public async Task<IActionResult> Details(int type, long id, long characterId)
        {
            var membershipType = (BungieMembershipType)type;
            _logger.LogInformation($"{membershipType}/{id}/{characterId}");

            var accessToken = await _contextAccessor.HttpContext.GetTokenAsync("access_token");

            var maxGear = await _maxPower.ComputeMaxPowerAsync(membershipType, id, characterId);
            if(maxGear == null)
            {
                _logger.LogWarning("Couldn't find max gear. Redirecting to Account Index");
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
                EmblemPath = _bungie.Value.BaseUrl + character.Character.Data.EmblemPath,
                EmblemBackgroundPath = _bungie.Value.BaseUrl + character.Character.Data.EmblemBackgroundPath
            };

            return View(model);
        }
    }
}