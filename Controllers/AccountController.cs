using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Destiny2;
using MaxPowerLevel.Models;
using MaxPowerLevel.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MaxPowerLevel.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IDestinyService _destiny;
        private readonly IManifestService _manifest;

        public AccountController(IDestinyService destiny, IManifestService manifest)
        {
            _destiny = destiny;
            _manifest = manifest;
        }

        [HttpGet("login")]
        public IActionResult Login(string returnUrl = "/")
        {
            return Challenge(new AuthenticationProperties() { RedirectUri = returnUrl });
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new AccountsViewModel();

            var value = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            long.TryParse(value, out long membershipId);

            var membershipData = await _destiny.GetMembershipDataAsync(membershipId);
            model.Accounts = (from membership in membershipData.Memberships
                                select new Account(membership.MembershipType, membership.MembershipId))
                                .ToList();

            return View(model);
        }

        [HttpGet("{type}/{id}")]
        public async Task<IActionResult> Details(int type, long id)
        {
            var membershipType = (BungieMembershipType)type;
            var model = new AccountDetailsViewModel(membershipType, id);

            var profileResponse = await _destiny.GetProfileAsync(membershipType, id);
            foreach(var characterId in profileResponse.Profile.Data.CharacterIds)
            {
                var characterInfo = await _destiny.GetCharacterInfoAsync(membershipType, id, characterId, DestinyComponentType.Characters);
                var classDef = await _manifest.LoadClassAsync(characterInfo.Character.Data.ClassHash);

                model.Characters.Add(new Character(characterId, characterInfo.Character.Data, classDef));
            }

            return View(model);
        }
    }
}