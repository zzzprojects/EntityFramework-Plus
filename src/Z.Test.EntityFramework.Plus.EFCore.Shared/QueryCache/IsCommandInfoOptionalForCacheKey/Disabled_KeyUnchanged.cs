// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class QueryCache_IsCommandInfoOptionalForCacheKey
    {
        [TestMethod]
        public void Disabled_KeyUnchanged()
        {
            Action action = () =>
            {
                TestContext.DeleteAll(x => x.Entity_Basics);
                TestContext.Insert(x => x.Entity_Basics, 3);

                using (var ctx = new TestContext())
                {
                    var firstTag = "zzzprojects";
                    var columnInt = 987654;

                    StringBuilder cacheKey = new StringBuilder();
                    cacheKey.AppendLine(QueryCacheManager.CachePrefix);
                    cacheKey.AppendLine(QueryCacheManager.GetConnectionStringForCacheKey(ctx.Entity_Basics));
                    cacheKey.AppendLine(firstTag);

                    var query = ctx.Entity_Basics.Where(x => x.ColumnInt > columnInt);

                    var cacheKey1 = QueryCacheManager.GetCacheKey(query, new[] {firstTag});

                    QueryCacheManager.IsCommandInfoOptionalForCacheKey = true;
                    QueryCacheManager.UseFirstTagAsCacheKey = true;

                    try
                    {
                        var cacheKey2 = QueryCacheManager.GetCacheKey(query, new[] {firstTag});

                        Assert.AreNotEqual(cacheKey1, cacheKey2);
                    }
                    finally
                    {
                        QueryCacheManager.UseFirstTagAsCacheKey = false;
                        QueryCacheManager.IsCommandInfoOptionalForCacheKey = false;
                    }

                    var cacheKey3 = QueryCacheManager.GetCacheKey(query, new[] {firstTag});

                    // The default cache key still contains the connection, the command and the parameter value
                    Assert.IsTrue(cacheKey1.StartsWith(cacheKey.ToString()));
                    Assert.IsTrue(cacheKey1.Contains(columnInt.ToString()));

                    // The default cache key is left untouched by the fast path
                    Assert.AreEqual(cacheKey1, cacheKey3);
                }
            };

            MyIni.RunWithFailLogical(MyIni.GetSetupCasTest(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name), action);
        }
    }
}