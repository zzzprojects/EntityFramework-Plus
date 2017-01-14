// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if !EFCORE
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;
using Z.EntityFramework.Plus.EF6.Cache.Redis;
using Z.EntityFramework.Plus.EF6.Cache.Redis.Factories;

namespace Z.Test.EntityFramework.Plus.EF6.Cache.Redis
{
    public partial class QueryCache_ExpireAll
    {
        [TestMethod]
        public void Queryable_WithTag()
        {
            var clientFactory = new StackExchangeRedisCacheClientFactory();

            QueryCacheManager.Cache = new RedisCacheProvider(clientFactory);
            QueryCacheManager.CacheTagProvider = new RedisCacheTagProvider(clientFactory);

            var testCacheKey = Guid.NewGuid().ToString();

            TestContext.DeleteAll(x => x.Entity_Basics);
            TestContext.Insert(x => x.Entity_Basics, 1);

            using (var ctx = new TestContext())
            {
                // BEFORE
                var itemCountBefore = ctx.Entity_Basics.FromCache(testCacheKey).Count();
                var cacheCountBefore = QueryCacheHelper.GetCacheCount();

                TestContext.DeleteAll(x => x.Entity_Basics);
                QueryCacheManager.ExpireAll();

                // EXPIRED
                var cacheCountExpired = QueryCacheHelper.GetCacheCount();

                // AFTER
                var itemCountAfter = ctx.Entity_Basics.FromCache(testCacheKey).Count();
                var cacheCountAfter = QueryCacheHelper.GetCacheCount();

                // TEST: The item count are not equal
                Assert.AreNotEqual(itemCountBefore, itemCountAfter);

                // TEST: The cache count are equal
                Assert.AreEqual(cacheCountBefore, cacheCountAfter);

                // TEST: The cache count after ExpireAll call is zero
                Assert.AreEqual(0, cacheCountExpired);
            }

            QueryCacheManager.ExpireAll();
        }
    }
}
#endif