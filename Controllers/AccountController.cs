using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Destiny2;
using MaxPowerLevel.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MaxPowerLevel.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;

        public AccountController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            return Challenge(new AuthenticationProperties() { RedirectUri = returnUrl });
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new AccountsViewModel();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            
            using(var destiny = new Destiny(_config["Bungie:ApiKey"], accessToken))
            {
                var value = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                long.TryParse(value, out long membershipId);

                var membershipData = await destiny.GetMembershipData(membershipId);
                model.Accounts = (from membership in membershipData.Memberships
                                  select new Account(membership.MembershipType, membership.MembershipId))
                                 .ToList();
            }

            return View(model);
        }

        [HttpGet("{type}/{id}")]
        public async Task<IActionResult> Details(int type, long id)
        {
            var membershipType = (BungieMembershipType)type;
            var model = new AccountDetailsViewModel(membershipType, id);

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            using(var destiny = new Destiny(_config["Bungie:ApiKey"], accessToken))
            {
                var profileResponse = await destiny.GetProfile(membershipType, id);
                foreach(var characterId in profileResponse.Profile.Data.CharacterIds)
                {
                    var characterInfo = await destiny.GetCharacterInfo(membershipType, id, characterId, DestinyComponentType.Characters);
                    model.Characters.Add(new Character(characterId, characterInfo.Character.Data));
                }
            }

            return View(model);
        }
    }
}