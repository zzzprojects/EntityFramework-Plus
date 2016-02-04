// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2016 ZZZ Projects. All rights reserved.

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class QueryCache_Tag
    {
        [TestMethod]
        public void Tag_NotEqual()
        {
            var testCacheKey = Guid.NewGuid().ToString();

            TestContext.DeleteAll(x => x.Entity_Basics);
            TestContext.Insert(x => x.Entity_Basics, 1);

            using (var ctx = new TestContext())
            {
                // BEFORE
                var itemCountBefore = ctx.Entity_Basics.FromCache(testCacheKey).Count();
                var cacheCountBefore = QueryCacheHelper.GetCacheCount();

                TestContext.DeleteAll(x => x.Entity_Basics);

                // AFTER
                var itemCountAfter = ctx.Entity_Basics.FromCache(testCacheKey, Guid.NewGuid().ToString()).Count();
                var cacheCountAfter = QueryCacheHelper.GetCacheCount();

                // TEST: The item count are NOT equal (A new cache key is used, the query is materialized)
                Assert.AreNotEqual(itemCountBefore, itemCountAfter);
                Assert.AreEqual(0, itemCountAfter);

                // TEST: The cache count are NOT equal (A new cache key is used)
                Assert.AreEqual(cacheCountBefore + 1, cacheCountAfter);
            }
        }
    }
}