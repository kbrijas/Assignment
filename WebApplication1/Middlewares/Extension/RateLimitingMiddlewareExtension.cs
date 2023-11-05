namespace WebApplication1.Middlewares.Extension
{
    public static class RateLimitingMiddlewareExtension
    {
        public static IApplicationBuilder UseRateLimiter(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RateLimitingMiddleware>();
        }
    }

    public class RateLimterMiddlewarePipeline
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseRateLimiter();
        }
    }
}
