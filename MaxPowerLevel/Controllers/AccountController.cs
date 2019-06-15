using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Destiny2;
using MaxPowerLevel.Helpers;
using MaxPowerLevel.Models;
using MaxPowerLevel.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace MaxPowerLevel.Controllers
{
  [Route("[controller]")]
  public class AccountController : Controller
  {
    private readonly IDestiny _destiny;
    private readonly IManifestService _manifest;
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IOptions<BungieSettings> _bungie;

    public AccountController(IDestiny destiny, IManifestService manifest, IConfiguration config,
        IHttpContextAccessor contextAccessor, IOptions<BungieSettings> bungie)
    {
        _destiny = destiny;
        _manifest = manifest;
        _config = config;
        _contextAccessor = contextAccessor;
        _bungie = bungie;
    }

    [HttpGet("login")]
    public IActionResult Login(string returnUrl = "/")
    {
        return Challenge(new AuthenticationProperties() { RedirectUri = returnUrl });
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(_config["BungieLoginCookieName"]);

        var url = Url.Action("Index", "Home");
        return Redirect(url);
    }

    [HttpGet(Name = "AccountIndex")]
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var accessToken = _contextAccessor.HttpContext.GetTokenAsync("access_token");

        var value = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        long.TryParse(value, out long membershipId);

        var membershipData = await _destiny.GetMembershipData(await accessToken, membershipId);
        var accounts = (from membership in membershipData.Memberships
                        select new Account(membership.MembershipType, membership.MembershipId))
                       .ToList();

        if (1 == accounts.Count)
        {
          // If there is only one account, redirect to the page for it.
          var url = Url.RouteUrl("AccountDetails", new
          {
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
        var accessToken = _contextAccessor.HttpContext.GetTokenAsync("access_token");

        var membershipType = (BungieMembershipType)type;
        var model = new AccountDetailsViewModel(membershipType, id);

        var profileResponse = await _destiny.GetProfile(await accessToken, membershipType, id, DestinyComponentType.Characters);
        if (profileResponse == null)
        {
            var url = Url.RouteUrl("AccountIndex");
            return Redirect(url);
        }

        foreach (var item in profileResponse.Characters.Data)
        {
            var classDef = await _manifest.LoadClassAsync(item.Value.ClassHash);
            model.Characters.Add(new Character(item.Key, item.Value, classDef, _bungie.Value.BaseUrl));
        }

        return View(model);
    }
  }
}