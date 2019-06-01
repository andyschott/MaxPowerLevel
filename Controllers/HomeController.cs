using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Destiny2;
using Microsoft.AspNetCore.Mvc;
using MaxPowerLevel.Models;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;

namespace MaxPowerLevel.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;

        public HomeController(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel();

            if(User.Identity.IsAuthenticated)
            {
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
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
