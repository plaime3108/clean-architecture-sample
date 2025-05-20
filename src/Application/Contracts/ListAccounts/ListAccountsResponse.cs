using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.AccountList
{
    public class ListAccountsResponse
    {
        public string TypeDirection { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public List<Account> Accounts { get; set; } = [];
    }
    public class Account
    {
        public decimal IdAccount { get; set; }
        public Guid IdAccountGuid { get; set; }
        public string UnformattedAccount { get; set; } = string.Empty;
        public string MaskedAccount { get; set; } = string.Empty;
        public string TextAccount { get; set; } = string.Empty;
        public short Currency { get; set; }
        public decimal Balance { get; set; }
    }
}
