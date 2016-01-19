// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class QueryCache_Tag
    {
        [TestMethod]
        public void Tag_Expire()
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

                QueryCacheManager.ExpireTag(testCacheKey);
                var cacheCountExpired = QueryCacheHelper.GetCacheCount();

                // TEST: The cache count are NOT equal (The cache key has been removed)
                Assert.AreEqual(cacheCountBefore - 1, cacheCountExpired);

                // AFTER
                var itemCountAfter = ctx.Entity_Basics.FromCache(testCacheKey).Count();
                var cacheCountAfter = QueryCacheHelper.GetCacheCount();

                // TEST: The item count are NOT equal (The query has been expired)
                Assert.AreNotEqual(itemCountBefore, itemCountAfter);
                Assert.AreEqual(0, itemCountAfter);

                // TEST: The cache count are NOT equal (The expired cache key is added)
                Assert.AreEqual(cacheCountExpired + 1, cacheCountAfter);
                Assert.AreEqual(cacheCountBefore, cacheCountAfter);
            }
        }
    }
}