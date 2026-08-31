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
    public partial class QueryCache_IsCommandInfoOptionalForCacheKey
    {
        [TestMethod]
        public void WithoutTagMode_Throws()
        {
            Action action = () =>
            {
                TestContext.DeleteAll(x => x.Entity_Basics);
                TestContext.Insert(x => x.Entity_Basics, 3);

                using (var ctx = new TestContext())
                {
                    var query = ctx.Entity_Basics.Where(x => x.ColumnInt > 0);

                    QueryCacheManager.IsCommandInfoOptionalForCacheKey = true;

                    try
                    {
                        var cacheKey = QueryCacheManager.GetCacheKey(query, new[] {"zzzprojects"});

                        Assert.Fail("The cache key should not have been created");
                    }
                    catch (AssertFailedException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Assert.AreEqual(ExceptionMessage.QueryCache_IsCommandInfoOptionalForCacheKey_Invalid, ex.Message);
                    }
                    finally
                    {
                        QueryCacheManager.IsCommandInfoOptionalForCacheKey = false;
                    }
                }
            };

            MyIni.RunWithFailLogical(MyIni.GetSetupCasTest(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name), action);
        }
    }
}