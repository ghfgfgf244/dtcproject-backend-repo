namespace dtc.Infrastructure.Configurations
{
    public class UpstashVectorSettings
    {
        public const string SectionName = "AI:UpstashVector";

        public string Endpoint { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string IndexName { get; set; } = "dtc-knowledge";
        public int DefaultTopK { get; set; } = 5;
    }
}
