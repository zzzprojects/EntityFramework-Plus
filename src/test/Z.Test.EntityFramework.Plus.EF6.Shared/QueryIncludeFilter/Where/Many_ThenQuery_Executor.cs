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
    public partial class QueryIncludeFilter_Where
    {
        [TestMethod]
        public void Many_ThenQuery_Executor()
        {
            TestContext.DeleteAll(x => x.Association_Multi_OneToMany_Right1s);
            TestContext.DeleteAll(x => x.Association_Multi_OneToMany_Right2s);
            TestContext.DeleteAll(x => x.Association_Multi_OneToMany_Lefts);

            using (var ctx = new TestContext())
            {
                {
                    var left = TestContext.Insert(ctx, x => x.Association_Multi_OneToMany_Lefts, 1).First();
                    left.Right1s = TestContext.Insert(ctx, x => x.Association_Multi_OneToMany_Right1s, 5);
                    left.Right2s = TestContext.Insert(ctx, x => x.Association_Multi_OneToMany_Right2s, 5);
                }
                {
                    var left = TestContext.Insert(ctx, x => x.Association_Multi_OneToMany_Lefts, 1).First();
                    left.ColumnInt = 5;
                    left.Right1s = TestContext.Insert(ctx, x => x.Association_Multi_OneToMany_Right1s, 5);
                    left.Right2s = TestContext.Insert(ctx, x => x.Association_Multi_OneToMany_Right2s, 5);
                }

                ctx.SaveChanges();
            }

            using (var ctx = new TestContext())
            {
                var item = ctx.Association_Multi_OneToMany_Lefts
                    .IncludeFilter(left => left.Right1s.Where(y => y.ColumnInt > 2))
                    .IncludeFilter(left => left.Right2s.Where(y => y.ColumnInt > 2))
                    .Where(x => x.ColumnInt < 5)
                    .OrderBy(x => x.ID)
                    .First();

                // TEST: context
                Assert.AreEqual(5, ctx.ChangeTracker.Entries().Count());

                // TEST: right1
                Assert.AreEqual(2, item.Right1s.Count);

                if (item.Right1s[0].ColumnInt == 3)
                {
                    Assert.AreEqual(3, item.Right1s[0].ColumnInt);
                    Assert.AreEqual(4, item.Right1s[1].ColumnInt);
                }
                else
                {
                    Assert.AreEqual(4, item.Right1s[0].ColumnInt);
                    Assert.AreEqual(3, item.Right1s[1].ColumnInt);
                }

                // TEST: right2
                Assert.AreEqual(2, item.Right2s.Count);

                if (item.Right2s[0].ColumnInt == 3)
                {
                    Assert.AreEqual(3, item.Right2s[0].ColumnInt);
                    Assert.AreEqual(4, item.Right2s[1].ColumnInt);
                }
                else
                {
                    Assert.AreEqual(4, item.Right2s[0].ColumnInt);
                    Assert.AreEqual(3, item.Right2s[1].ColumnInt);
                }
            }
        }
    }
}