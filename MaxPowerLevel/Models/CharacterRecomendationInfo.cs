using System;
using System.Collections.Generic;
using Destiny2;

namespace MaxPowerLevel.Models
{
    public class CharacterRecomendationInfo
    {
        public IEnumerable<Item> Items { get; set; }

        private decimal _powerLevel = 0M;
        public decimal PowerLevel
        {
            get { return _powerLevel; }
            set
            {
                _powerLevel = value;
                IntPowerLevel = (int)Math.Floor(_powerLevel);
            }
        }
        public int IntPowerLevel { get; private set; }
        public IDictionary<uint, DestinyProgression> Progressions { get; set; }
    }
}
