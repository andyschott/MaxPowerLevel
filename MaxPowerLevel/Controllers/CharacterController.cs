using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Destiny2;
using Destiny2.Entities.Items;
using MaxPowerLevel.Models;
using MaxPowerLevel.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MaxPowerLevel.Controllers
{
    [Route("[controller]")]
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
            var model = new CharacterViewModel()
            {
                Items = maxGear,
                MaxPower = _maxPower.ComputePower(maxGear.Values),
            };

            return View(model);
        }
    }
}