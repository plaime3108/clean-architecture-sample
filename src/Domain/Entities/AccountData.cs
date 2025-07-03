namespace Domain.Entities
{
    public class AccountData
    {
        public DateTime openingDate { get; set; }
        public DateTime lastTransactionDate { get; set; }
        public decimal availableBalance { get; set; }
        public decimal totalBalance { get; set; }
        public decimal retaindedBalance { get; set; }
        public decimal GarnishmentBalance { get; set; }
        public decimal PledgedBalance { get; set; }
        public decimal BlockedBalance { get; set; }
        public decimal InterestRate { get; set; }
    }
}
