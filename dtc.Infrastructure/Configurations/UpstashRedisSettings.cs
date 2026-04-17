namespace dtc.Infrastructure.Configurations
{
    public class UpstashRedisSettings
    {
        public const string SectionName = "AI:UpstashRedis";

        public string RestUrl { get; set; } = string.Empty;
        public string RestToken { get; set; } = string.Empty;
        public string KeyPrefix { get; set; } = "dtc:ai";
    }
}
