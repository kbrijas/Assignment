using Microsoft.Extensions.Options;
using System.Diagnostics.Metrics;
using System.Text.Json;
using System.Threading;
using WebApplication1.Middlewares.Models;
using WebApplication1.Middlewares.Options;
using WebApplication1.Middlewares.Storage;
using WebApplication1.Middlewares.ThreadSaafe;

namespace WebApplication1.Middlewares.Validators
{
    public class RateLimiter : IRateLimiter
    {
        private readonly RateLimitingOptions _rateLimitOptions;
        private IStorage _storage; 

        public RateLimiter(IOptions<RateLimitingOptions> rateLimitConfig, IStorage storage)
        {
            _rateLimitOptions = rateLimitConfig.Value ?? throw new ArgumentNullException(nameof(rateLimitConfig)); ;
            _storage = storage;
        }


        public async Task<bool> IsRequestAllowedAsync(string userId)
        {
            //Lock mechanism key based take from stack overflow
            KeyedLock<string> locker = new KeyedLock<string>();

            using (await locker.WaitAsync(userId).ConfigureAwait(true))
            {
                var entryTime = DateTime.UtcNow;
                int maxLimit = _rateLimitOptions.RequestLimit;
                int timeInMinutes = _rateLimitOptions.ValidityMins;

                //For First Time Entry
                var userRateData = new RateLimiterUserData()
                {
                    Timestamp = entryTime,
                    Count = 1
                };

                var userUniqueKey = $"{_rateLimitOptions.CacheKey}_{userId}";
                var savedKeyData = await _storage.GetAsync(userUniqueKey);

                //First Time Call
                //Set New entry for user
                if (savedKeyData == null)
                {
                    var stringifiedData = JsonSerializer.Serialize(userRateData);
                    await _storage.SetAsync(userUniqueKey, stringifiedData);
                    return true;
                }

                //Logic if Some Data exist
                var data = JsonSerializer.Deserialize<RateLimiterUserData>(savedKeyData);
                if (data?.Timestamp.AddMinutes(timeInMinutes) >= DateTime.UtcNow)
                {
                    //if user exceed more than limit in duration from the firstrequest
                    if (data.Count >= maxLimit)
                    {
                        return false;
                    }

                    // increment onther 1 Count
                    var totalCount = data.Count + 1;

                    userRateData = new RateLimiterUserData
                    {
                        Timestamp = data.Timestamp,
                        Count = totalCount
                    };

                    var stringifiedData = JsonSerializer.Serialize(userRateData);
                    await _storage.SetAsync(userUniqueKey, stringifiedData);
                    return true;
                }
                else
                {
                    //Time Expired Condition.. 
                    // Restart
                    await _storage.DeleteAsync(userUniqueKey);
                    var reinsert = JsonSerializer.Serialize(userRateData);
                    await _storage.SetAsync(userUniqueKey, reinsert);
                    return true;
                }
            }
        }

        public async Task<bool> IsWhitelistedAsync(string userId)
        {
            //To Exclude the White Listed Clients from rate limiting Mentioned in App Settings 
            if (_rateLimitOptions.WhiteListedClient != null && _rateLimitOptions.WhiteListedClient.Contains(userId))
            {
                return true;
            }

            return false;
        }
    }
}
