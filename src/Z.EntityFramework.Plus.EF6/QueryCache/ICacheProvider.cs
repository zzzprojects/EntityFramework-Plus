using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Z.EntityFramework.Plus.QueryCache
{
    public interface ICacheProvider
    {
        IDictionary<string, object> GetAll();
        object Get(string key);
        object AddOrGetExisting(string key, object item, CacheItemPolicy policy);
        object AddOrGetExisting(string key, object item, DateTimeOffset absoluteExpiration);
        void Remove(string key);
        long GetCount();
    }

    public class MemoryCacheProvider : ICacheProvider
    {
        private readonly ObjectCache cache;

        public MemoryCacheProvider(MemoryCache cache = null)
        {
            this.cache = cache ?? MemoryCache.Default;
        }

        public IDictionary<string, object> GetAll()
        {
            return cache.ToDictionary(x => x.Key, x => x.Value);
        }

        public object Get(string key)
        {
            return cache.Get(key);
        }

        public object AddOrGetExisting(string key, object item, CacheItemPolicy policy)
        {
            return cache.AddOrGetExisting(key, item, policy);
        }

        public object AddOrGetExisting(string key, object item, DateTimeOffset absoluteExpiration)
        {
            return cache.AddOrGetExisting(key, item, absoluteExpiration);
        }

        public void Remove(string key)
        {
            cache.Remove(key);
        }

        public long GetCount()
        {
            return cache.GetCount();
        }
    }
}
