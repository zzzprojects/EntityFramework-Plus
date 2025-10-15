// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class QueryCache_CacheKey
    {
        [TestMethod]
        public void Immediate_Constant()
        {
            TestContext.DeleteAll(x => x.Entity_Basics);
            TestContext.Insert(x => x.Entity_Basics, 3);

            using (var ctx = new TestContext())
            {
                var query1 = ctx.Entity_Basics.Where(x => x.ColumnInt > 0);
                var query2 = ctx.Entity_Basics.Where(x => x.ColumnInt > 1);

                var count1 = query1.FromCache().Count();
                var count2 = query2.FromCache().Count();

                // Cache key are different
                Assert.AreNotEqual(QueryCacheManager.GetCacheKey(query1, new string[0]), QueryCacheManager.GetCacheKey(query2, new string[0]));

                // Count are different
                Assert.AreEqual(2, count1);
                Assert.AreEqual(1, count2);
            }
        }
    }
}