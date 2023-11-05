namespace WebApplication1.Middlewares.Options
{
    public class RateLimitingOptions
    {
        public const string Config = "RateLimitingConfig";
        public int RequestLimit { get; set; }
        public List<string>? WhiteListedClient { get; set; }
        public int ValidityMins { get; set; }
        public string? CacheKey { get; set; }
    }
}
