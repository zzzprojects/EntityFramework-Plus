using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Z.EntityFramework.Plus.QueryCache
{
    public class MemoryCacheProvider : ICacheProvider
    {
        private readonly ObjectCache _cache;

        public MemoryCacheProvider(MemoryCache cache = null)
        {
            _cache = cache ?? MemoryCache.Default;
        }

        public IDictionary<string, object> GetAll()
        {
            return _cache.ToDictionary(x => x.Key, x => x.Value);
        }

        public object Get(string key)
        {
            return _cache.Get(key);
        }

        public object AddOrGetExisting(string key, object item, CacheItemPolicy policy)
        {
            return _cache.AddOrGetExisting(key, item, policy);
        }

        public object AddOrGetExisting(string key, object item, DateTimeOffset absoluteExpiration)
        {
            return _cache.AddOrGetExisting(key, item, absoluteExpiration);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public long GetCount()
        {
            return _cache.GetCount();
        }
    }
}