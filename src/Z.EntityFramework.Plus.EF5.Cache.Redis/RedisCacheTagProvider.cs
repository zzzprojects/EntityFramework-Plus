using System.Linq;
using StackExchange.Redis.Extensions.Core;
using Z.EntityFramework.Plus.EF5.Cache.Redis.Factories.Interfaces;
using Z.EntityFramework.Plus.QueryCache;

namespace Z.EntityFramework.Plus.EF5.Cache.Redis
{
    public class RedisCacheTagProvider : ICacheTagProvider
    {
        private readonly ICacheClient _client;

        public RedisCacheTagProvider(IStackExchangeRedisCacheClientFactory redisCacheClientFactory)
        {
            _client = redisCacheClientFactory.Get();
        }

        public void AddOrUpdate(string cacheKey, string fullTag)
        {
            var list = _client.Get<string[]>(GetKey(fullTag));

            if (list == null)
                list = new []{ cacheKey };
            else if (!list.Contains(cacheKey))
                list = list.Concat(new [] { cacheKey }).ToArray();

            _client.Add(GetKey(fullTag), list);
        }

        public IRemoveTagResult Remove(string fullTag)
        {
            var list = _client.Get<string[]>(GetKey(fullTag));

            var hasRemoved = _client.Remove(GetKey(fullTag));

            return new RemoveTagResult(hasRemoved, list);
        }

        private string GetKey(string fullTag)
        {
            return $"tags.{fullTag}";
        }
    }
}
