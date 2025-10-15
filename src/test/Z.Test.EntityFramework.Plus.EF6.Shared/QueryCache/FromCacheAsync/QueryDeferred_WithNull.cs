// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if EF5 || EF6
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class QueryCache_FromCacheAsync
    {
        [TestMethod]
        public void FromCacheAsync_QueryDeferred_WithNull()
        {
            using (var ctx = new TestContext())
            {
                // GET a null item from database
                {
                    var item = ctx.Entity_Basics.DeferredFirstOrDefault(x => x.ID == -1).FromCacheAsync().Result;

                    Assert.IsNull(item);
                }

                // GET a null item from cache
                {
                    var item = ctx.Entity_Basics.DeferredFirstOrDefault(x => x.ID == -1).FromCacheAsync().Result;

                    Assert.IsNull(item);
                }
            }
        }
    }
}

#endif