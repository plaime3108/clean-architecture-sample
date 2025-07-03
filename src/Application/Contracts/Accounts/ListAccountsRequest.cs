namespace Application.Contracts.AccountList
{
    public class ListAccountsRequest
    {
        public short Channel { get; set; }
        public short CountryDocument { get; set; }
        public short DocumentType { get; set; }
        public required string DocumentNumber { get; set; }
        public string Complement { get; set; } = string.Empty;
        public string Issue { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public short Channel { get; set; }
        public string TraceNumber { get; set; } = string.Empty;
        public int IdCnf { get; set; }
        public string AccountType { get; set; } = string.Empty;
    }
}
