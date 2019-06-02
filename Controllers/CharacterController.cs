using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Destiny2;
using Destiny2.Entities.Items;
using MaxPowerLevel.Models;
using MaxPowerLevel.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MaxPowerLevel.Controllers
{
    [Route("[controller]")]
    public class CharacterController : Controller
    {
        private readonly IDestinyService _destiny;

        private static readonly ISet<ItemSlot> _includedSlots =
            new HashSet<ItemSlot>
            {
                ItemSlot.KineticWeapon,
                ItemSlot.EnergyWeapon,
                ItemSlot.PowerWeapon,
                ItemSlot.Helmet,
                ItemSlot.Gauntlet,
                ItemSlot.ChestArmor,
                ItemSlot.LegArmor,
                ItemSlot.ClassArmor,
            };

        public CharacterController(IDestinyService destiny)
        {
            _destiny = destiny;
        }
        
        [HttpGet("{type}/{id}/{characterId}")]
        public async Task<IActionResult> Details(int type, long id, long characterId)
        {
            var membershipType = (BungieMembershipType)type;

            var db = new ManifestDb((string)HttpContext.Items["ManifestDbPath"]);
            var model = new CharacterViewModel();

            var characterInfo = await _destiny.GetCharacterInfoAsync(membershipType, id, characterId, DestinyComponentType.CharacterEquipment);
            foreach(var itemComponent in characterInfo.Equipment.Data.Items)
            {
                if(!_includedSlots.Contains((ItemSlot)itemComponent.BucketHash))
                {
                    continue;
                }

                var item = await db.LoadInventoryItem(itemComponent.ItemHash);
                DestinyItemInstanceComponent instance = null;
                if(item.Inventory.IsInstanceItem)
                {
                    var instanceResponse = await _destiny.GetItemAsync(membershipType, id, itemComponent.ItemInstanceId, DestinyComponentType.ItemInstances);
                    instance = instanceResponse?.Instance?.Data;
                }
                model.Items.Add(new Item(itemComponent, item, instance));
            }
            
            return View(model);
        }
    }
}