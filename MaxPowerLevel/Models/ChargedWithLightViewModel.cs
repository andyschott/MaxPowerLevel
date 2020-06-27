using System.Collections.Generic;
using Destiny2;

namespace MaxPowerLevel.Models
{
    public class ChargedWithLightViewModel
    {
        public BungieMembershipType AccountType { get; set; }
        public long AccountId { get; set; }
        public IEnumerable<ModData> BecomeCharged { get; set; }
        public IEnumerable<ModData> WhileCharged { get; set; }
    }
}