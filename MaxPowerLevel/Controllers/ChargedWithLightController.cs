using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Destiny2;
using Destiny2.Definitions;
using MaxPowerLevel.Models;
using MaxPowerLevel.Services;
using Microsoft.AspNetCore.Mvc;

namespace MaxPowerLevel.Controllers
{
    [Route("chargedWithLight")]
    public class ChargedWithLightController : Controller
    {
        private readonly IManifest _manifest;
        private readonly ChargedWithLight _chargedWithLight;
        private static readonly ISet<uint> _modCategoryHashes = new HashSet<uint>
        {
            1362265421, //[]
            3872696960, // Gauntlets
            3723676689, // Chest
            3607371986, // Legs
        };
        private const uint ModsCategory = 59;
        private const uint ArmorModsCategory = 4104513227;

        public ChargedWithLightController(IManifest manifest, ChargedWithLight chargedWithLight)
        {
            _manifest = manifest;
            _chargedWithLight = chargedWithLight;
        }
        
        [Route("{type}/{id}")]
        public async Task<IActionResult> Index(BungieMembershipType type, long id)
        {
            var chargedWithLightMods = await _chargedWithLight.LoadMods(type, id);
            return View(new ChargedWithLightViewModel
            {
                AccountType = type,
                AccountId = id,
                BecomeCharged = chargedWithLightMods[ChargedWithLightType.Become].OrderBy(mod => mod.Name),
                WhileCharged = chargedWithLightMods[ChargedWithLightType.While].OrderBy(mod => mod.Name)
            });
        }
    }
}