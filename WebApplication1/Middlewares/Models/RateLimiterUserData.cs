namespace WebApplication1.Middlewares.Models
{
    public class RateLimiterUserData
    {
        public DateTime Timestamp { get; set; }

        public double Count { get; set; }
    }
}
