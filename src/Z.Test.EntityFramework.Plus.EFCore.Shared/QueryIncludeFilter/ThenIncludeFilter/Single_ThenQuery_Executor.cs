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
    public partial class QueryIncludeFilter_ThenIncludeFilter
    {
        [TestMethod]
        public void Single_ThenQuery_Executor()
        {
            TestContext.DeleteAll(x => x.Association_OneToMany_Rights);
            TestContext.DeleteAll(x => x.Association_OneToMany_Lefts);

            using (var ctx = new TestContext())
            {
                {
                    var left = TestContext.Insert(ctx, x => x.Association_OneToMany_Lefts, 1).First();
                    left.Rights = TestContext.Insert(ctx, x => x.Association_OneToMany_Rights, 5);
                }
                {
                    var left = TestContext.Insert(ctx, x => x.Association_OneToMany_Lefts, 1).First();
                    left.ColumnInt = 5;
                    left.Rights = TestContext.Insert(ctx, x => x.Association_OneToMany_Rights, 5);
                }
                ctx.SaveChanges();
            }

            using (var ctx = new TestContext())
            {
                var item = ctx.Association_OneToMany_Lefts
                    .IncludeFilter(left => left.Rights.Where(y => y.ColumnInt > 2))
                    .Where(x => x.ColumnInt < 5)
                    .OrderBy(x => x.ID)
                    .First();

                // TEST: context
                Assert.AreEqual(3, ctx.ChangeTracker.Entries().Count());

                // TEST: right
                Assert.AreEqual(2, item.Rights.Count);

                if (item.Rights[0].ColumnInt == 3)
                {
                    Assert.AreEqual(3, item.Rights[0].ColumnInt);
                    Assert.AreEqual(4, item.Rights[1].ColumnInt);
                }
                else
                {
                    Assert.AreEqual(4, item.Rights[0].ColumnInt);
                    Assert.AreEqual(3, item.Rights[1].ColumnInt);
                }
            }
        }
    }
}