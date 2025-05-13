namespace Infrastructure.Configurations
{
    public class RedisConfig
    {
        public const string SectionName = "Redis";
        public string Hostname { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
