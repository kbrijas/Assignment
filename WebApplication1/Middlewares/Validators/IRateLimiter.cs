namespace WebApplication1.Middlewares.Validators
{
    public interface IRateLimiter
    {
        Task<bool> IsRequestAllowedAsync(string userId);
        Task<bool> IsWhitelistedAsync(string userId);
        // Define other necessary methods...
    }
}
