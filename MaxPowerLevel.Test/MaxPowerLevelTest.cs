 using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Destiny2;
using MaxPowerLevel.Helpers;
using MaxPowerLevel.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MaxPowerLevel.Test
{
    [TestClass]
    public class UnitTest1
    {
        private static IDictionary<string, Item> _items = CreateItems();

        [TestMethod]
        public void AllLegendaries()
        {
            var items = CreateItems("Blast Furnace", "Breakneck", "Midnight Coup",
                "IKELOS SG", "Nation of Beasts", "Loaded Question",
                "Edge Transit", "Avalanche", "Outrageous Fortune");
            var actual = FindMax(items);
            var expected = new[]
            {
                _items["Breakneck"],
                _items["Loaded Question"],
                _items["Edge Transit"]
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void TwoExotics()
        {
            var items = CreateItems("Blast Furnace", "Breakneck", "Midnight Coup",
                "MIDA Multi-Tool", "IKELOS SG", "Nation of Beasts", "Loaded Question",
                "Riskrunner", "Edge Transit", "Avalanche", "Outrageous Fortune");
            var actual = FindMax(items);
            var expected = new[]
            {
                _items["MIDA Multi-Tool"],
                _items["Loaded Question"],
                _items["Edge Transit"]
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        private static IDictionary<string, Item> CreateItems()
        {
            var items = new[]
            {
                new Item("Blast Furnace", ItemSlot.SlotHashes.Kinetic, 653),
                new Item("Breakneck", ItemSlot.SlotHashes.Kinetic, 660),
                new Item("Midnight Coup", ItemSlot.SlotHashes.Kinetic, 380),
                new Item("MIDA Multi-Tool", ItemSlot.SlotHashes.Kinetic, 700, TierType.Exotic),

                new Item("IKELOS SG", ItemSlot.SlotHashes.Energy, 380),
                new Item("Nation of Beasts", ItemSlot.SlotHashes.Energy, 650),
                new Item("Loaded Question", ItemSlot.SlotHashes.Energy, 690),
                new Item("Riskrunner", ItemSlot.SlotHashes.Energy, 700, TierType.Exotic),

                new Item("Edge Transit", ItemSlot.SlotHashes.Power, 700),
                new Item("Avalanche", ItemSlot.SlotHashes.Power, 698),
                new Item("Outrageous Fortune", ItemSlot.SlotHashes.Power, 600),
                new Item("Whisper of the Worm", ItemSlot.SlotHashes.Power, 380, TierType.Exotic),
            };

            return items.ToDictionary(Item => Item.Name);
        }

        private static ILookup<ItemSlot.SlotHashes, Item> CreateItems(params string[] names)
        {
            var items = names.Select(name => _items[name])
                .OrderByDescending(item => item.PowerLevel)
                .ToLookup(item => item.Slot.Hash);
            return items;
        }

        private ICollection FindMax(ILookup<ItemSlot.SlotHashes, Item> items)
        {
            return MaxPower.FindMax(items[ItemSlot.SlotHashes.Kinetic],
                items[ItemSlot.SlotHashes.Energy],
                items[ItemSlot.SlotHashes.Power]) as ICollection;
        }
    }
}
