using System.Collections.Generic;
using Destiny2;

namespace MaxPowerLevel.Models
{
    public class AccountDetailsViewModel
    {
        public AccountDetailsViewModel(BungieMembershipType type, long id)
        {
            Type = type;
            Id = id;
        }

        public BungieMembershipType Type { get; }
        public long Id { get; }
        public IList<Character> Characters { get; set; } = new List<Character>();
    }
}