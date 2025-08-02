using ExchangeRatesApi.Services.Interfaces;
using System.Collections.Concurrent;

namespace ExchangeRatesApi.Services
{
    internal class DailyCacheService : ICacheService
    {
        private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();
        public DailyCacheService() 
        {   }

        public T? GetData<T>(string key) where T : class
        {
            var today = DateTime.Now.Date;
            if(_cache.TryGetValue(key, out var cacheEntry) && cacheEntry.Date == today)
            {
                return cacheEntry.Data as T;
            }
            return null;
        }

        public void AddData(string key, object data)
        {
            var today = DateTime.Now.Date;

            CacheEntry cacheEntry = new CacheEntry() { Date = today, Data = data };
            _cache.AddOrUpdate(key, (key) => cacheEntry, (key, oldValue) => cacheEntry);
       }

        private class CacheEntry
        {
            public object Data { get; set; }
            public DateTime Date { get; set; } = DateTime.MinValue;
        }
    }
}
