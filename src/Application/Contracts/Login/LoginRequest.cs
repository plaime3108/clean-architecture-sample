namespace Application.Contracts.Login
{
    public class LoginRequest
    {
        public int IdCnf { get; set; }
        public short CountryDocument { get; set; }
        public short DocumentType { get; set; }
        public required string DocumentNumber { get; set; }
        public string Complement { get; set; } = string.Empty;
        public string Issue { get; set; } = string.Empty;
    }
}
