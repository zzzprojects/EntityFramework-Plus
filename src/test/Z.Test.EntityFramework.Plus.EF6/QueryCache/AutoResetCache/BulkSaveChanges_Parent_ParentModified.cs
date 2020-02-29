// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class QueryCache_AutoResetCache
    {
        [TestMethod]
        public void BulkSaveChanges_Parent_ParentModified()
        {
            TestContext.DeleteAll(x => x.Association_OneToMany_Rights);
            TestContext.DeleteAll(x => x.Association_OneToMany_Lefts);

            using (var ctx = new TestContext())
            {
                var left = ctx.Association_OneToMany_Lefts.Add(new Association_OneToMany_Left() { ColumnInt = 1 });

                ctx.SaveChanges();
            }

            using (var ctx = new TestContext())
            {
                QueryCacheManager.IsAutoExpireCacheEnabled = true;

                // BEFORE
                var before = ctx.Association_OneToMany_Lefts.Include("Rights").FromCache().ToList();

                ctx.Association_OneToMany_Lefts.ToList().ForEach(x => x.ColumnInt++);
                ctx.BulkSaveChanges();

                var after = ctx.Association_OneToMany_Lefts.Include("Rights").FromCache().ToList();

                QueryCacheManager.ExpireAll();
                QueryCacheManager.IsAutoExpireCacheEnabled = false;

                // TEST: The item count are equal
                Assert.AreEqual(1, before.First().ColumnInt);
                Assert.AreEqual(0, before.First().Rights.Count);

                // TEST: The cache count are equal
                Assert.AreEqual(2, after.First().ColumnInt);
                Assert.AreEqual(0, after.First().Rights.Count);
            }
        }
    }
}