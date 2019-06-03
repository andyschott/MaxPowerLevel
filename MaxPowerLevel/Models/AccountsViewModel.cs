using System.Collections.Generic;
using System.Linq;
using Destiny2.Config;
using MaxPowerLevel.Helpers;

namespace MaxPowerLevel.Models
{
    public class AccountsViewModel
    {
        public IEnumerable<Account> Accounts { get; set; }

        public string GetAccountName(Account account)
        {
            return Utilities.GetDescription(account.Type);
        }
    }
}