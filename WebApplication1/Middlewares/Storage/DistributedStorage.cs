using Microsoft.Extensions.Caching.Distributed;

namespace WebApplication1.Middlewares.Storage
{

    public class DistributedStorage : IStorage
    {
        private readonly IDistributedCache _cache;

        public DistributedStorage(IDistributedCache cache)
        {
            _cache = cache;
        }

        async Task IStorage.DeleteAsync(string id) => _cache.Remove(id);

        async Task<string> IStorage.GetAsync(string id) => await _cache.GetStringAsync(id);

        async Task IStorage.SetAsync(string id, string data)
        {
            var options = new DistributedCacheEntryOptions();
            await _cache.SetStringAsync(id, data, options);
        }
    }
}
