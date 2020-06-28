using System.Collections.Generic;

namespace MaxPowerLevel.Models
{
    public class ModData
    {
        public uint Hash { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Perks { get; set; }
        public string Type { get; set; }
        public string IconUrl { get; set; }
        public ChargedWithLightType? ChargedWithLightType { get; set; }
        public ModElement Element { get; set; }
        public bool IsUnlocked { get; set; }

        public override string ToString() => Name;
    }
}