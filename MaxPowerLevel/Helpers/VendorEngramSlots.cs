using System.Collections.Generic;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Helpers
{
    public static class VendorEngramSlots
    {
        private const uint BansheeHash = 672118013;

        private static readonly ItemSlot.SlotHashes[] _weaponSlots = new[]
        {
            ItemSlot.SlotHashes.Kinetic,
            ItemSlot.SlotHashes.Energy,
            ItemSlot.SlotHashes.Power
        };

        private static readonly ItemSlot.SlotHashes[] _allGearSlots = new[]
        {
            ItemSlot.SlotHashes.Kinetic,
            ItemSlot.SlotHashes.Energy,
            ItemSlot.SlotHashes.Power,

            ItemSlot.SlotHashes.Helmet,
            ItemSlot.SlotHashes.Gauntlet,
            ItemSlot.SlotHashes.ChestArmor,
            ItemSlot.SlotHashes.LegArmor,
            ItemSlot.SlotHashes.ClassArmor
        };

        public static IEnumerable<ItemSlot.SlotHashes> GetEngramSlots(uint vendorHash)
        {
            if(vendorHash == BansheeHash)
            {
                // Banshee's engrams only contain weapons.
                return _weaponSlots;
            }

            // All other vendor engrams contain all slots.
            return _allGearSlots;
        }
    }
}