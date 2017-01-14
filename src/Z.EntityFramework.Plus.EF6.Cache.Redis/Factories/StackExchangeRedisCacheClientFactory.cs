using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;
using Z.EntityFramework.Plus.EF6.Cache.Redis.Factories.Interfaces;

namespace Z.EntityFramework.Plus.EF6.Cache.Redis.Factories
{
    public class StackExchangeRedisCacheClientFactory : IStackExchangeRedisCacheClientFactory
    {
        private ICacheClient client;

        public ICacheClient Get()
        {
            if (client != null)
                return client;

            return client = new StackExchangeRedisCacheClient(new NewtonsoftSerializer(
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.All
                }));
        }
    }
}
