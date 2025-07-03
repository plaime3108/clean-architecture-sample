namespace Application.Contracts.AccountList
{
    public class ListAccountsResponse
    {
        public string ClientName { get; set; } = string.Empty;
        public IEnumerable<AccountsNode> Accounts { get; set; } = [];
    }

    public class AccountsNode
    {
        public string TypeDirection { get; set; } = string.Empty;
        public List<AccountResponse> AccountNode { get; set; } = [];
    }
    public class AccountResponse
    {
        public decimal IdAccount { get; set; }
        public Guid IdAccountGuid { get; set; }
        public string UnformattedAccount { get; set; } = string.Empty;
        public string MaskedAccount { get; set; } = string.Empty;
        public string TextAccount { get; set; } = string.Empty;
        public short Currency { get; set; }
        public int Balance { get; set; }
        public int DebtAmount { get; set; }
        public int DebtAmountConv { get; set; }
        public short EndCurrency { get; set; }
    }
}
