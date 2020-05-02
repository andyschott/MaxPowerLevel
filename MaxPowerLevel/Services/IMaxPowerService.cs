using System.Collections.Generic;
using System.Threading.Tasks;
using Destiny2;
using Destiny2.Entities;
using Destiny2.Entities.Items;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services
{
    public interface IMaxPowerService
    {
         Task<IDictionary<ItemSlot.SlotHashes, Item>> ComputeMaxPower(DestinyCharacterComponent character,
            IEnumerable<DestinyInventoryComponent> characterEquipment,
            IEnumerable<DestinyInventoryComponent> characterInventories,
            DestinyInventoryComponent vault,
            IDictionary<long, DestinyItemInstanceComponent> itemInstances);
        
         Task<IDictionary<long, IDictionary<ItemSlot.SlotHashes, Item>>> ComputeMaxPowerAsync(IDictionary<long, DestinyCharacterComponent> characters,
            IEnumerable<DestinyInventoryComponent> characterEquipment,
            IEnumerable<DestinyInventoryComponent> characterInventories,
            DestinyInventoryComponent vault,
            IDictionary<long, DestinyItemInstanceComponent> itemInstances);

         decimal ComputePower(IEnumerable<Item> items);
    }
}