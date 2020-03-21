using System.Collections.Generic;
using System.Linq;

namespace MaxPowerLevel.Models
{
    public class Recommendation
    {
        public string Description { get; }
        public IEnumerable<string> Activities { get; }

        public Recommendation(string description)
            : this(description, null)
        {
        }

        public Recommendation(string description, IEnumerable<string> activities)
        {
            Description = description;
            Activities = activities ?? Enumerable.Empty<string>();
        }

        public override string ToString() => Description;
    }
}