using Microsoft.AspNetCore.Http;
using System.Net;
using WebApplication1.Middlewares.Validators;

namespace WebApplication1.Middlewares
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRateLimiter _rateLimiter;

        public RateLimitingMiddleware(RequestDelegate next, IRateLimiter rateLimiter)
        {
            _next = next;
           _rateLimiter = rateLimiter;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Assuming application dedployed in windows authentication
            // First Process is to identify the user
            var userId = context.User?.Identity?.Name;
            if (userId is null) // Try tp resolve with ip addess
            {
                userId = context.Connection.RemoteIpAddress?.ToString(); 
            }

            // Check if the user is in whitelisted configuration stored in app settings
            if (await _rateLimiter.IsWhitelistedAsync(userId))
            {
                await _next.Invoke(context);
                return;
            }

            //Check timeperiod , Max limit reached
            if (!await _rateLimiter.IsRequestAllowedAsync(userId))
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.ContentType = "text/plain";
                return;
            }

            await _next(context);
        }
    }
}
