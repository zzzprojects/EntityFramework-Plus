using StackExchange.Redis.Extensions.Core;

namespace Z.EntityFramework.Plus.EF5.Cache.Redis.Factories.Interfaces
{
    public interface IStackExchangeRedisCacheClientFactory
    {
        ICacheClient Get();
    }
}
