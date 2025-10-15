// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class QueryCache_UseTagsAsCacheKey
    {
        [TestMethod]
        public void Single()
        {
            TestContext.DeleteAll(x => x.Entity_Basics);
            TestContext.Insert(x => x.Entity_Basics, 3);

            using (var ctx = new TestContext())
            {
                var firstTag = "zzzprojects";

                StringBuilder cacheKey = new StringBuilder();
                cacheKey.AppendLine(QueryCacheManager.CachePrefix);
                cacheKey.AppendLine(QueryCacheManager.GetConnectionStringForCacheKey(ctx.Entity_Basics));
                cacheKey.AppendLine(firstTag);

                var query = ctx.Entity_Basics.Where(x => x.ColumnInt > 0);

                var cacheKey1 = QueryCacheManager.GetCacheKey(query, new string[0]);
                QueryCacheManager.UseTagsAsCacheKey = true;
                var cacheKey2 = QueryCacheManager.GetCacheKey(query, new[] {firstTag});
                QueryCacheManager.UseTagsAsCacheKey = false;

                // Cache key are different
                Assert.AreNotEqual(cacheKey1, cacheKey2);

                // Cache key2 is equal to hardcoded cacheKey
                Assert.AreEqual(cacheKey.ToString(), cacheKey2);
            }
        }
    }
}