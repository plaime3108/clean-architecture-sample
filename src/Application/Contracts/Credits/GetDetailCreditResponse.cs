namespace Application.Contracts.Credits
{
    public class GetDetailCreditResponse
    {
        public int TotalInstallment { get; set; }
        public int Principalbalance { get; set; }
        public short Currency { get; set; }
        public int LegalExpenses { get; set; }
        public string AccountId { get; set; } = string.Empty;
        public int AmountBs { get; set; }
        public int AmountUsd { get; set; }
        public short Endcurrency { get; set; }
        public IEnumerable<InstallmentNode> Installments { get; set; } = [];
    }

    public class InstallmentNode
    {
        public int Installment { get; set; }
        public string DueDate { get; set; } = string.Empty;
        public string InstallmentType { get; set; } = string.Empty;
        public int Principal { get; set; }
        public int Interest { get; set; }
        public int CurrentInterest { get; set; }
        public int DeferredInterest { get; set; }
        public int Insurance { get; set; }
        public int CompensatoryInterest { get; set; }
        public int InstallmentFee { get; set; }
    }
}
