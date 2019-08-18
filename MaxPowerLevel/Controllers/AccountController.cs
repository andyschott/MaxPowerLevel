using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Destiny2;
using MaxPowerLevel.Helpers;
using MaxPowerLevel.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MaxPowerLevel.Controllers
{
  [Route("[controller]")]
  public class AccountController : Controller
  {
    private readonly IDestiny2 _destiny;
    private readonly IManifest _manifest;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IOptions<BungieSettings> _bungie;
    private readonly ILogger _logger;

    public AccountController(IDestiny2 destiny, IManifest manifest,
        IHttpContextAccessor contextAccessor, IOptions<BungieSettings> bungie,
        ILogger<AccountController> logger)
    {
        _destiny = destiny;
        _manifest = manifest;
        _contextAccessor = contextAccessor;
        _bungie = bungie;
        _logger = logger;
    }

    [HttpGet("login")]
    public IActionResult Login(string returnUrl = "/")
    {
        _logger.LogInformation("Login");
        return Challenge(new AuthenticationProperties() { RedirectUri = returnUrl });
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        _logger.LogInformation("Logut");
        Response.Cookies.Delete(_bungie.Value.LoginCookieName);

        var url = Url.Action("Index", "Home");
        return Redirect(url);
    }

    [HttpGet(Name = "AccountIndex")]
    [Authorize]
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Index");
        var accessToken = _contextAccessor.HttpContext.GetTokenAsync("access_token");

        var value = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        long.TryParse(value, out long membershipId);

        var linkedProfiles = await _destiny.GetLinkedProfiles(await accessToken, membershipId);

        var accounts = linkedProfiles.Profiles.Select(profile => new Account(profile.MembershipType, profile.MembershipId))
                                              .ToList();

        if (1 == accounts.Count)
        {
          _logger.LogInformation("Only one account - redirecting to account page");

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
        var membershipType = (BungieMembershipType)type;
        _logger.LogInformation($"{membershipType}/{id}");

        var accessToken = _contextAccessor.HttpContext.GetTokenAsync("access_token");

        var model = new AccountDetailsViewModel(membershipType, id);

        var profileResponse = await _destiny.GetProfile(await accessToken, membershipType, id, DestinyComponentType.Characters);
        if (profileResponse == null)
        {
            var url = Url.RouteUrl("AccountIndex");
            return Redirect(url);
        }

        foreach (var item in profileResponse.Characters.Data)
        {
            var classDef = await _manifest.LoadClass(item.Value.ClassHash);
            model.Characters.Add(new Character(item.Key, item.Value, classDef, _bungie.Value.BaseUrl));
        }

        return View(model);
    }
  }
}