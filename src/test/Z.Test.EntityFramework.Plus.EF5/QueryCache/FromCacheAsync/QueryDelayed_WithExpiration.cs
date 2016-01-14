// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

#if EF5 || EF6
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class QueryCache_FromCacheAsync
    {
        [TestMethod]
        public void FromCacheAsync_QueryDelayed_WithExpirations()
        {
            TestContext.DeleteAll(x => x.Entity_Basics);
            TestContext.Insert(x => x.Entity_Basics, 1);

            using (var ctx = new TestContext())
            {
                // BEFORE
                var itemCountBefore = ctx.Entity_Basics.DeferredCount().FromCacheAsync(DateTime.Now.AddMinutes(2)).Result;
                var cacheCountBefore = QueryCacheHelper.GetCacheCount();

                TestContext.DeleteAll(x => x.Entity_Basics);

                // AFTER
                var itemCountAfter = ctx.Entity_Basics.DeferredCount().FromCacheAsync(DateTime.Now.AddMinutes(2)).Result;
                var cacheCountAfter = QueryCacheHelper.GetCacheCount();

                // TEST: The item count are equal
                Assert.AreEqual(itemCountBefore, itemCountAfter);

                // TEST: The cache count are equal
                Assert.AreEqual(cacheCountBefore, cacheCountAfter);
            }
        }
    }
}

#endif