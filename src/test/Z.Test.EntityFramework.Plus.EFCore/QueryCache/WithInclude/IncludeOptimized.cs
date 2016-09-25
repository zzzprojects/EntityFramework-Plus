// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.
#if !EFCORE
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class QueryCache_WithInclude
    {
        [TestMethod]
        public void IncludeOptimized()
        {
            TestContext.DeleteAll(x => x.Association_OneToMany_Rights);
            TestContext.DeleteAll(x => x.Association_OneToMany_Lefts);

            using (var ctx = new TestContext())
            {
                var left = ctx.Association_OneToMany_Lefts.Add(new Association_OneToMany_Left());
                var right = ctx.Association_OneToMany_Rights.Add(new Association_OneToMany_Right());

                left.Rights = new List<Association_OneToMany_Right>();
                left.Rights.Add(right);

                ctx.SaveChanges();
            }

            using (var ctx = new TestContext())
            {
                // BEFORE
                var itemCountBefore = ctx.Association_OneToMany_Lefts.IncludeOptimized(x => x.Rights).FromCache().Count();
                var cacheCountBefore = QueryCacheHelper.GetCacheCount();

                TestContext.DeleteAll(x => x.Association_OneToMany_Rights);
                TestContext.DeleteAll(x => x.Association_OneToMany_Lefts);

                // AFTER
                var itemCountAfter = ctx.Association_OneToMany_Lefts.IncludeOptimized(x => x.Rights).FromCache().Count();
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