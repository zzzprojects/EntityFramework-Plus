// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class QueryIncludeOptimized_FirstWithPredicate
    {
        [TestMethod]
        public void Single_ThenQuery_Executor()
        {
            TestContext.DeleteAll(x => x.Association_OneToMany_Lefts);
            TestContext.DeleteAll(x => x.Association_OneToMany_Rights);

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
                    .IncludeOptimized(left => left.Rights.Where(y => y.ColumnInt > 2))
                    .Where(x => x.ColumnInt < 5)
                    .OrderBy(x => x.ID)
                    .First(x => x.ColumnInt < 10);

                // TEST: context
                Assert.AreEqual(3, ctx.ChangeTracker.Entries().Count());

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