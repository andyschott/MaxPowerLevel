using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Destiny2;
using Destiny2.Entities.Items;
using MaxPowerLevel.Models;
using MaxPowerLevel.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MaxPowerLevel.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class CharacterController : Controller
    {
        private readonly IDestinyService _destiny;
        private readonly IManifestService _manifest;
        private readonly IMaxPowerService _maxPower;

        public CharacterController(IDestinyService destiny, IManifestService manifest, IMaxPowerService maxPower)
        {
            _destiny = destiny;
            _manifest = manifest;
            _maxPower = maxPower;
        }
        
        [HttpGet("{type}/{id}/{characterId}")]
        public async Task<IActionResult> Details(int type, long id, long characterId)
        {
            var membershipType = (BungieMembershipType)type;

            var maxGear = await _maxPower.ComputeMaxPowerAsync(membershipType, id, characterId);
            if(maxGear == null)
            {
                var url = Url.RouteUrl("AccountIndex");
                return Redirect(url);
            }
            var character = await _destiny.GetCharacterInfoAsync(membershipType, id, characterId, DestinyComponentType.Characters);

            var model = new CharacterViewModel()
            {
                Type = membershipType,
                AccountId = id,
                Items = maxGear.Values,
                MaxPower = _maxPower.ComputePower(maxGear.Values),
                EmblemPath = Destiny.BaseAddress + character.Character.Data.EmblemPath,
                EmblemBackgroundPath = Destiny.BaseAddress + character.Character.Data.EmblemBackgroundPath
            };

            return View(model);
        }
    }
}