using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using StackExchange.Redis.Extensions.Core;
using Z.EntityFramework.Plus.EF5.Cache.Redis.Factories.Interfaces;
using Z.EntityFramework.Plus.QueryCache;

namespace Z.EntityFramework.Plus.EF5.Cache.Redis
{
    public class RedisCacheProvider : ICacheProvider
    {
        private readonly ICacheClient _client;
        private readonly string _prefix;

        public RedisCacheProvider(
            IStackExchangeRedisCacheClientFactory redisCacheClientFactory,
            string prefix = "Z.EntityFramework.Plus.QueryCacheManager;")
        {
            _client = redisCacheClientFactory.Get();
            _prefix = prefix;
        }

        public IDictionary<string, object> GetAll()
        {
            var keys = _client.SearchKeys($"{_prefix}*");
            return _client.GetAll<object>(keys);
        }

        public object Get(string key)
        {
            var item = _client.Get<object>(key);

            return item;
        }

        public object AddOrGetExisting(string key, object item, DateTimeOffset absoluteExpiration)
        {
            var result = Get(key);

            if (result != null)
                return result;

            _client.Add(key, item, absoluteExpiration);

            return item;
        }

        public void Remove(string key)
        {
            _client.Remove(key);
        }

        public long GetCount()
        {
            return _client.SearchKeys($"{_prefix}*").Count();
        }

        public object AddOrGetExisting(string key, object item, CacheItemPolicy policy)
        {
            return AddOrGetExisting(key, item, policy.AbsoluteExpiration);
        }
    }
}
