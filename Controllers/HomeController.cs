using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Destiny2;
using Microsoft.AspNetCore.Mvc;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            using(var destiny = new Destiny())
            {
                var manifest = await destiny.GetManifest();
                var model = new HomeViewModel()
                {
                    Manifest = manifest
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
