// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class BatchDelete_ActionAsync
    {
        [TestMethod]
        public async Task TwentyAsync()
        {
            TestContext.DeleteAll(x => x.Entity_Basics);
            TestContext.Insert(x => x.Entity_Basics, 50);

            using (var ctx = new TestContext())
            {
                // BEFORE
                Assert.AreEqual(1225, ctx.Entity_Basics.Sum(x => x.ColumnInt));

                // ACTION
                var rowsAffected = await ctx.Entity_Basics.Where(x => x.ColumnInt > 10 && x.ColumnInt <= 30).DeleteAsync();

                // AFTER
                Assert.AreEqual(815, ctx.Entity_Basics.Sum(x => x.ColumnInt));
                Assert.AreEqual(20, rowsAffected);
            }
        }
    }
}