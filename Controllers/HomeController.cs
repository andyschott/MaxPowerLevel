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

namespace MaxPowerLevel.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;

        public HomeController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            using(var destiny = new Destiny(_config["Bungie:ApiKey"]))
            {
                int membershipId = -1;
                if(User.Identity.IsAuthenticated)
                {
                    var value = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                    int.TryParse(value, out membershipId);
                }

                var model = new HomeViewModel()
                {
                    MembershipId = membershipId
                };
                return View(model);
            }
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
