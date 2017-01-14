using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Z.EntityFramework.Plus.QueryCache
{
    public class InMemoryCacheTagProvider : ICacheTagProvider
    {
        private readonly ConcurrentDictionary<string, List<string>> _cacheTags;

        public InMemoryCacheTagProvider()
        {
            _cacheTags = new ConcurrentDictionary<string, List<string>>();
        }

        public void AddOrUpdate(string cacheKey, string fullTag)
        {
            _cacheTags.AddOrUpdate(fullTag, x => new List<string> { cacheKey }, (x, list) =>
            {
                if (!list.Contains(cacheKey))
                {
                    list.Add(cacheKey);
                }

                return list;
            });
        }

        public IRemoveTagResult Remove(string fullTag)
        {
            List<string> list;

            var success = _cacheTags.TryRemove(fullTag, out list);

            return new RemoveTagResult(success, list);
        }
    }
}