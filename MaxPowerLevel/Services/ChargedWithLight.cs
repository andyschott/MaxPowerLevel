using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Destiny2;
using Destiny2.Definitions;
using MaxPowerLevel.Helpers;
using MaxPowerLevel.Models;
using Microsoft.Extensions.Options;

namespace MaxPowerLevel.Services
{
    public class ChargedWithLight
    {
        private readonly IManifest _manifest;
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

        public ChargedWithLight(IManifest manifest, IOptions<BungieSettings> bungie)
        {
            _manifest = manifest;
            _baseUrl = bungie.Value.BaseUrl;
        }
        
        public async Task<ILookup<ChargedWithLightType?, ModData>> LoadMods()
        {
            var armorMods = await _manifest.LoadInventoryItemsWithCategory(ArmorModsCategory);
            var modData = await LoadMods(armorMods);

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

        private Task<ModData[]> LoadMods(IEnumerable<DestinyInventoryItemDefinition> mods)
        {
            var modDataTasks = mods.Where(mod => mod.Perks.Any())
                .Select(async mod =>
                {
                    var perks = (await _manifest.LoadSandboxPerks(mod.Perks.Select(perk => perk.PerkHash)))
                        .Where(perk => perk.IsDisplayable);
                    return new ModData
                    {
                        Hash = mod.Hash,
                        Name = mod.DisplayProperties.Name,
                        Type = mod.ItemTypeDisplayName,
                        Perks = perks.Select(perk => perk.DisplayProperties.Description).ToArray(),
                        IconUrl = BuildIconUrl(mod),
                        ChargedWithLightType = GetChargedWithLightType(perks)
                    };
                });

            return Task.WhenAll(modDataTasks);
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
    }
}