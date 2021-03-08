using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Destiny2;
using Destiny2.Entities.Items;
using MaxPowerLevel.Helpers;
using MaxPowerLevel.Models;
using MaxPowerLevel.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MaxPowerLevel.Controllers
{
    [Authorize]
    public class CharacterController : Controller
    {
        private readonly IDestiny2 _destiny;
        private readonly IManifest _manifest;
        private readonly IMaxPowerService _maxPower;
        private readonly IRecommendations _recommendations;
        private readonly Affinitization _affinitization;
        private readonly ItemService _itemService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOptions<BungieSettings> _bungie;
        private readonly ILogger _logger;

        public CharacterController(IDestiny2 destiny, IMaxPowerService maxPower,
            IManifest manifest, IRecommendations recommendations,
            Affinitization affinitization, ItemService itemService,
            IHttpContextAccessor contextAccessor, IOptions<BungieSettings> bungie,
            ILogger<CharacterController> logger)
        {
            _destiny = destiny;
            _maxPower = maxPower;
            _manifest = manifest;
            _recommendations = recommendations;
            _affinitization = affinitization;
            _itemService = itemService;
            _contextAccessor = contextAccessor;
            _bungie = bungie;
            _logger = logger;
        }

        [HttpGet("Character/{type}/{id}/{characterId}")]
        public IActionResult CharacterDetailsRedirect(int type, long id, long characterId)
        {
            var url = Url.Action("Details", new
            {
               type = type,
               id = id,
               characterId = characterId 
            });

            return Redirect(url);
        }        

        [HttpGet("{type}/{id}/characters/{characterId}")]
        public async Task<IActionResult> Details(int type, long id, long characterId)
        {
            _affinitization.SetCookies(Request.Cookies);

            var membershipType = (BungieMembershipType)type;
            _logger.LogInformation($"{membershipType}/{id}/{characterId}");

            var accessToken = await _contextAccessor.HttpContext.GetTokenAsync("access_token");

            var profileTask = _destiny.GetProfile(accessToken, membershipType, id,
                DestinyComponentType.ProfileInventories, DestinyComponentType.Characters,
                DestinyComponentType.CharacterInventories, DestinyComponentType.CharacterEquipment,
                DestinyComponentType.ItemInstances, DestinyComponentType.ProfileProgression);
            var characterProgressionsTask = _destiny.GetCharacterInfo(accessToken, membershipType, id, characterId,
                DestinyComponentType.Characters, DestinyComponentType.CharacterProgressions);

            await Task.WhenAll(profileTask, characterProgressionsTask);

            var profile = profileTask.Result;
            var characterProgressions = characterProgressionsTask.Result;

            if(!profile.Characters.Data.TryGetValue(characterId, out var character))
            {
                _logger.LogWarning($"Could not find character {characterId}");
                return null;
            }

            var maxGear = await _maxPower.ComputeMaxPower(character,
                profile.CharacterEquipment.Data.Values,
                profile.CharacterInventories.Data.Values,
                profile.ProfileInventory.Data,
                profile.ItemComponents.Instances.Data);
            if(maxGear == null)
            {
                _logger.LogWarning("Couldn't find max gear. Redirecting to Account Index");
                var url = Url.RouteUrl("AccountIndex");
                return Redirect(url);
            }

            var inventory = Enumerable.Empty<DestinyItemComponent>();
            if(profile.CharacterInventories.Data.TryGetValue(characterId, out var inventoryComponent))
            {
                inventory = inventoryComponent.Items;
            }
            var engrams = await _itemService.GetEngrams(inventory, profile.ItemComponents.Instances.Data);

            var lowestItems = _maxPower.FindLowestItems(maxGear.Values).ToList();

            var classTask = _manifest.LoadClass(character.ClassHash);

            var maxPower = _maxPower.ComputePower(maxGear.Values);
            var recommendationsTask = _recommendations.GetRecommendations(new CharacterRecomendationInfo
            {
                Items = maxGear.Values,
                PowerLevel = maxPower,
                Progressions = characterProgressions.Progressions.Data.Progressions,
                Milestones = characterProgressions.Progressions.Data.Milestones,
                Engrams = engrams
            });

            await Task.WhenAll(classTask, recommendationsTask);

            var emblemPath = string.Empty;
            var emblemBackgroundPath = string.Empty;
            if(character.EmblemBackgroundPath != null)
            {
                emblemPath = _bungie.Value.BaseUrl + character.EmblemPath;
                emblemBackgroundPath = _bungie.Value.BaseUrl + character.EmblemBackgroundPath;
            }
            
            var model = new CharacterViewModel()
            {
                Type = membershipType,
                AccountId = id,
                Id = characterId,
                Items = maxGear.Values,
                LowestItems = lowestItems,
                BasePower = maxPower,
                BonusPower = profile.ProfileProgression.Data.SeasonalArtifact.PowerBonus,
                EmblemPath = emblemPath,
                EmblemBackgroundPath = emblemBackgroundPath,
                Recommendations = recommendationsTask.Result,
                Engrams = _recommendations.GetEngramPowerLevels(maxPower),
                ClassName = classTask.Result.DisplayProperties.Name
            };

            foreach(var cookie in _affinitization.GetCookies())
            {
                Response.Cookies.Append(cookie.name, cookie.value);
            }

            return View(model);
        }
    }
}