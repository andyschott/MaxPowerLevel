using System.Collections.Generic;
using System.Linq;

namespace MaxPowerLevel.Models
{
    public class ModData
    {
        public uint Hash { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string IconUrl { get; set; }

        public override string ToString() => Name;
    }
}