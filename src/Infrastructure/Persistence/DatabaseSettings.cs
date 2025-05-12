
namespace Infrastructure.Persistence
{
    public class DatabaseSettings
    {
        public const string SectionName = "Database";
        public string Server { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
        public string GetConnectionString()
        {
            return $"Server={Server};Database={Database};Integrated Security=true;TrustServerCertificate=true;";
        }
    }
}
