namespace Application.Contracts.Accounts
{
    public class AccountTransactionsResponse
    {
        public List<Transaction> Transactions { get; set; } = [];
        public string AccountNumber { get; set; } = string.Empty;
        public short Currency { get; set; }
        public int Balance { get; set; }
    }
    public class Transaction
    {
        public string Date { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public int Amount { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public string TransactionDetail { get; set; } = string.Empty;
    }
}
