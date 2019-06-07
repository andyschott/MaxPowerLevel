using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Destiny2;
using MaxPowerLevel.Models;
using MaxPowerLevel.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
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
        [Authorize]
        public async Task<IActionResult> Index()
        {

            var value = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            long.TryParse(value, out long membershipId);

            var membershipData = await _destiny.GetMembershipDataAsync(membershipId);
            var accounts = (from membership in membershipData.Memberships
                                select new Account(membership.MembershipType, membership.MembershipId))
                                .ToList();

            if(1 == accounts.Count)
            {
                // If there is only one account, redirect to the page for it.
                var url = Url.RouteUrl("AccountDetails", new {
                    type = (int)accounts[0].Type,
                    id = accounts[0].Id
                });
                return Redirect(url);
            }

            var model = new AccountsViewModel();
            model.Accounts = accounts;
            return View(model);
        }

        [HttpGet("{type}/{id}", Name = "AccountDetails")]
        [Authorize]
        public async Task<IActionResult> Details(int type, long id)
        {
            var membershipType = (BungieMembershipType)type;
            var model = new AccountDetailsViewModel(membershipType, id);

            var profileResponse = await _destiny.GetProfileAsync(membershipType, id, DestinyComponentType.Characters);
            foreach(var item in profileResponse.Characters.Data)
            {
                var classDef = await _manifest.LoadClassAsync(item.Value.ClassHash);
                model.Characters.Add(new Character(item.Key, item.Value, classDef));
            }

            return View(model);
        }
    }
}