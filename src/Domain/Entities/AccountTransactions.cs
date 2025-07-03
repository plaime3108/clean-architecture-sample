namespace Domain.Entities
{
    public class AccountTransactions
    {
        public DateTime Date { get; set; }
        public string Time { get; set; } = string.Empty;
        public double Amount { get; set; }
        public short TransactionType { get; set; }
        public string TransactionDetail { get; set; } = string.Empty;
    }
}
