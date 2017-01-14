using StackExchange.Redis.Extensions.Core;

namespace Z.EntityFramework.Plus.EF6.Cache.Redis.Factories.Interfaces
{
    public interface IStackExchangeRedisCacheClientFactory
    {
        ICacheClient Get();
    }
}
