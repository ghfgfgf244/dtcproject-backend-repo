using System.Collections.Generic;

namespace dtc.Infrastructure.Configurations
{
    public class GeminiSettings
    {
        public const string SectionName = "AI:Gemini";

        public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com";
        public List<string> ApiKeys { get; set; } = [];
        public int CooldownMinutesWhenRateLimited { get; set; } = 60;
    }
}
