using System;
using System.Collections.Generic;
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
}
