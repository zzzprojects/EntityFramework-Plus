// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class QueryCache_ForceFirstTagAsCacheKey
    {
        [TestMethod]
        public void None()
        {
            TestContext.DeleteAll(x => x.Entity_Basics);
            TestContext.Insert(x => x.Entity_Basics, 3);

            using (var ctx = new TestContext())
            {
                var firstTag = "zzzprojects";

                var query = ctx.Entity_Basics.Where(x => x.ColumnInt > 0);

                var cacheKey1 = QueryCacheManager.GetCacheKey(query, new string[0]);
                QueryCacheManager.ForceFirstTagAsCacheKey = true;

                try
                {
                    var cacheKey2 = QueryCacheManager.GetCacheKey(query, new string[0]);
                }
                catch (Exception ex)
                {
                    Assert.AreEqual(ExceptionMessage.QueryCache_FirstTagNullOrEmpty, ex.Message);
                }
                finally
                {
                    QueryCacheManager.ForceFirstTagAsCacheKey = false;
                }
            }
        }
    }
}