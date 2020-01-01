using System.Collections.Generic;
using System.Linq;

namespace MaxPowerLevel.Models
{
    public class Recommendation
    {
        public string Description { get; }
        public IEnumerable<string> Activities { get; }

        public Recommendation(string description)
            : this(description, Enumerable.Empty<string>())
        {
        }

        public Recommendation(string description, IEnumerable<string> activities)
        {
            Description = description;
            Activities = activities;
        }

        public override string ToString() => Description;
    }
}