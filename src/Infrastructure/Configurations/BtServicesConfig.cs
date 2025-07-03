namespace Infrastructure.Configurations
{
    public class BtServicesConfig
    {
        public const string SectionName = "BtServices";
        public string HttpClientName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public short Timeout { get; set; }
        public string BaseUrl { get; set; } = string.Empty;
        public string GetTokenPath { get; set; } = string.Empty;
        public string GetCreditAmountPath { get; set; } = string.Empty;
        public string GetPendingInstallmentPath { get; set; } = string.Empty;
    }
}
