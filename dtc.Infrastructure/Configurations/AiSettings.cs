namespace dtc.Infrastructure.Configurations
{
    public class AiSettings
    {
        public const string SectionName = "AI";

        public string DefaultModel { get; set; } = "gemma-3-27b-it";
        public string FallbackModel { get; set; } = "gemini-2.5-flash";
        public int DefaultTimeoutSeconds { get; set; } = 30;
        public bool EnableMockResponses { get; set; } = true;
        public int CacheMinutes { get; set; } = 10;
    }
}
