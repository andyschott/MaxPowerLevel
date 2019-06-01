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
                                    select new Account
                                    {
                                        Id = membership.MembershipId,
                                        Type = membership.MembershipType
                                    }).ToList();
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            return View();
        }
    }
}