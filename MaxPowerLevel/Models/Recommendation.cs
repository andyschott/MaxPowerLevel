using System.Collections.Generic;
using System.Linq;

namespace MaxPowerLevel.Models
{
    public class Recommendation
    {
        public string Description { get; }
        public IEnumerable<IEnumerable<string>> Activities { get; }

        public Recommendation(string description)
            : this(description, null)
        {
        }

        public Recommendation(string description, IEnumerable<IEnumerable<string>> activities)
        {
            Description = description;
            Activities = activities ?? Enumerable.Empty<IEnumerable<string>>();
        }

        public override string ToString() => Description;
    }
}