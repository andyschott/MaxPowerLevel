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

        public ChargedWithLight(IManifest manifest, IOptions<BungieSettings> bungie)
        {
            _manifest = manifest;
            _baseUrl = bungie.Value.BaseUrl;
        }
        
        public async Task<(IEnumerable<ModData> becomeCharged, IEnumerable<ModData> whileCharged)> LoadMods()
        {
            var armorMods = await _manifest.LoadInventoryItemsWithCategory(ArmorModsCategory);
            var modData = await LoadMods(armorMods);

            var becomeCharged = modData.Where(mod => mod.Description.Contains(Become));
            var whileCharged = modData.Where(mod => mod.Description.Contains(While));

            return (becomeCharged, whileCharged);
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
                    var perk = await _manifest.LoadSandboxPerk(mod.Perks.FirstOrDefault().PerkHash);
                    return new ModData
                    {
                        Hash = mod.Hash,
                        Name = mod.DisplayProperties.Name,
                        Type = mod.ItemTypeDisplayName,
                        Description = perk.DisplayProperties.Description,
                        IconUrl = BuildIconUrl(mod)
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
    }
}