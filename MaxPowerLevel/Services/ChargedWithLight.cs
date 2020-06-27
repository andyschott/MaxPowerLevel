using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Destiny2;
using Destiny2.Definitions;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services
{
    public class ChargedWithLight
    {
        private readonly IManifest _manifest;

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

        public ChargedWithLight(IManifest manifest)
        {
            _manifest = manifest;
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
                        Description = perk.DisplayProperties.Description
                    };
                });

            return Task.WhenAll(modDataTasks);
        }
    }
}