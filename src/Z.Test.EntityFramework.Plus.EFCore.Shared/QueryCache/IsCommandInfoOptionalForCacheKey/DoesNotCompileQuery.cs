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
        public void DoesNotCompileQuery()
        {
            Action action = () =>
            {
                TestContext.DeleteAll(x => x.Entity_Basics);
                TestContext.Insert(x => x.Entity_Basics, 3);

                using (var ctx = new TestContext())
                {
                    var firstTag = "zzzprojects";

                    StringBuilder cacheKey = new StringBuilder();
                    cacheKey.AppendLine(QueryCacheManager.CachePrefix);
                    cacheKey.AppendLine(firstTag);

                    // The query cannot be translated, so creating the cache key fails whenever the query is compiled
                    var query = ctx.Entity_Basics.Where(x => CannotBeTranslated(x.ColumnInt));

                    Exception compileException = null;

                    try
                    {
                        var cacheKey1 = QueryCacheManager.GetCacheKey(query, new[] {firstTag});
                    }
                    catch (Exception ex)
                    {
                        compileException = ex;
                    }

                    // The query is compiled by default
                    Assert.IsNotNull(compileException);

                    QueryCacheManager.SkipCommandCreationForCacheKey = true;
                    QueryCacheManager.UseFirstTagAsCacheKey = true;

                    try
                    {
                        // The query is not compiled anymore
                        var cacheKey2 = QueryCacheManager.GetCacheKey(query, new[] {firstTag});

                        Assert.AreEqual(cacheKey.ToString(), cacheKey2);
                    }
                    finally
                    {
                        QueryCacheManager.UseFirstTagAsCacheKey = false;
                        QueryCacheManager.SkipCommandCreationForCacheKey = false;
                    }
                }
            };

            MyIni.RunWithFailLogical(MyIni.GetSetupCasTest(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name), action);
        }

        private static bool CannotBeTranslated(int value)
        {
            return value > 0;
        }
    }
}