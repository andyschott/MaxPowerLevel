using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Destiny2;
using Destiny2.Components.Collectibles;
using Destiny2.Definitions;
using MaxPowerLevel.Helpers;
using MaxPowerLevel.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace MaxPowerLevel.Services
{
    public class ChargedWithLight
    {
        private readonly IDestiny2 _destiny;
        private readonly IManifest _manifest;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly string _baseUrl;

        private static readonly ISet<uint> _modCategoryHashes = new HashSet<uint>
        {
            1362265421, // Helmet
            3872696960, // Gauntlets
            3723676689, // Chest
            3607371986, // Legs
            3196106184, // Class Item
        };
        private const uint ArmorModsCategory = 4104513227;

        private const string Become = "Become Charged with Light";
        private const string While = "While Charged with Light";
        private const string ChargedWithLightText = "Charged with Light";

        public ChargedWithLight(IDestiny2 destiny, IManifest manifest,
         IHttpContextAccessor contextAccessor, IOptions<BungieSettings> bungie)
        {
            _destiny = destiny;
            _manifest = manifest;
            _contextAccessor = contextAccessor;
            _baseUrl = bungie.Value.BaseUrl;
        }
        
        public async Task<ILookup<ChargedWithLightType?, ModData>> LoadMods(BungieMembershipType type, long accountId)
        {
            var accessToken = await _contextAccessor.HttpContext.GetTokenAsync("access_token");

            var armorModsTask = LoadArmorMods();
            var profileTask = _destiny.GetProfile(accessToken, type, accountId,
                DestinyComponentType.Collectibles);

            await Task.WhenAll(armorModsTask, profileTask);

            var modData = await LoadMods(armorModsTask.Result,
                profileTask.Result.ProfileCollectibles.Data.Collectibles);

            return modData.ToLookup(mod => mod.ChargedWithLightType);
        }

        private async Task<IEnumerable<DestinyInventoryItemDefinition>> LoadArmorMods()
        {
            var armorMods = await _manifest.LoadInventoryItemsWithCategory(ArmorModsCategory);
            armorMods = armorMods.Where(mod =>
            {
                if(_modCategoryHashes.Overlaps(mod.ItemCategoryHashes))
                {
                    return true;
                }

                // All mods have at least the Mod category and ArmorMod category.
                // Mods with only those categories are the "General Armor Mods".
                // Return those only.
                return mod.ItemCategoryHashes.Count() == 2;
            });

            return armorMods;
        }

        private async Task<IEnumerable<ModData>> LoadMods(IEnumerable<DestinyInventoryItemDefinition> mods,
            IDictionary<uint, DestinyCollectibleComponent> collectibles)
        {
            var investmentStatHashes = mods.SelectMany(mod => mod.InvestmentStats)
                .Select(investmentStat => investmentStat.StatTypeHash)
                .Distinct();
            var investmentStatsTask = LoadStatTypes(investmentStatHashes);
            var perksTask = LoadPerks(mods);

            await Task.WhenAll(investmentStatsTask, perksTask);
            var investmentStats = investmentStatsTask.Result;
            var perks = perksTask.Result;

            var modData = mods.Where(mod => mod.Perks.Any())
                .Select(mod =>
                {
                    var isUnlocked = false;
                    if(mod.CollectibleHash != null)
                    {
                        if(collectibles.TryGetValue(mod.CollectibleHash.Value, out var collectible))
                        {
                            isUnlocked = collectible.State != DestinyCollectibleState.NotAcquired;
                        }
                    }

                    var modPerks = GetModPerks(mod, perks);

                    return new ModData
                    {
                        Hash = mod.Hash,
                        Name = mod.DisplayProperties.Name,
                        Type = mod.ItemTypeDisplayName,
                        Perks = modPerks.Select(perk => perk.DisplayProperties.Description).ToArray(),
                        IconUrl = BuildIconUrl(mod),
                        ChargedWithLightType = GetChargedWithLightType(modPerks),
                        Element = LoadStat(investmentStats, mod),
                        IsUnlocked = isUnlocked
                    };
                });

            return modData;
        }

        private async Task<IDictionary<uint, DestinySandboxPerkDefinition>> LoadPerks(IEnumerable<DestinyInventoryItemDefinition> mods)
        {
            var perkHashes = mods.SelectMany(mod => mod.Perks)
                .Select(perk => perk.PerkHash);
            var perks = await _manifest.LoadSandboxPerks(perkHashes);

            return perks.Where(perk => perk.IsDisplayable)
                .ToDictionary(perk => perk.Hash);
        }

        private IEnumerable<DestinySandboxPerkDefinition> GetModPerks(DestinyInventoryItemDefinition mod, IDictionary<uint, DestinySandboxPerkDefinition> perks)
        {
            var perkHashes = mod.Perks.Select(perk => perk.PerkHash);
            foreach(var perkHash in perkHashes)
            {
                if(perks.TryGetValue(perkHash, out var perk))
                {
                    yield return perk;
                }
            }
        }

        private string BuildIconUrl(DestinyInventoryItemDefinition item)
        {
            if(!item.DisplayProperties.HasIcon)
            {
                return string.Empty;
            }

            // The second icon in the sequence is the small icon.
            var smallIconUrl = item.DisplayProperties.IconSequences?.ElementAtOrDefault(1)
                ?.Frames.FirstOrDefault();

            if(string.IsNullOrEmpty(smallIconUrl))
            {
                return _baseUrl + item.DisplayProperties.Icon;
            }

            return _baseUrl + smallIconUrl;
        }

        private static ChargedWithLightType? GetChargedWithLightType(IEnumerable<DestinySandboxPerkDefinition> perks)
        {
            var chargedWithLight = perks.Select(perk => perk.DisplayProperties.Description)
                .Where(description => description.Contains(ChargedWithLightText));
            foreach(var description in chargedWithLight)
            {
                if(description.Contains(Become))
                {
                    return ChargedWithLightType.Become;
                }

                if(description.Contains(While))
                {
                    return ChargedWithLightType.While;
                }
            }
            return null;
        }

        private ModElement LoadStat(IDictionary<uint, DestinyStatDefinition> cache, DestinyInventoryItemDefinition item)
        {
            // assume mods have 1 investment stat
            var investmentStat = item.InvestmentStats.FirstOrDefault();
            if (investmentStat == null)
            {
                return ModElement.General;
            }

            var stat = cache[investmentStat.StatTypeHash];
            return stat.DisplayProperties.Name switch
            {
                "Arc Cost" => ModElement.Arc,
                "Solar Cost" => ModElement.Solar,
                "Void Cost" => ModElement.Void,
                _ => ModElement.General
            };
        }

        private async Task<IDictionary<uint, DestinyStatDefinition>> LoadStatTypes(IEnumerable<uint> hashes)
        {
            var statTypes = await _manifest.LoadStatTypes(hashes);
            return statTypes.ToDictionary(statTypes => statTypes.Hash);
        }

        private static ModElement GetElement(DestinyStatDefinition statDefinition)
        {
            return statDefinition.DisplayProperties.Name switch
            {
                "Arc Cost" => ModElement.Arc,
                "Solar Cost" => ModElement.Solar,
                "Void Cost" => ModElement.Void,
                _ => ModElement.General
            };
        }
    }
}