// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: http://zzzprojects.uservoice.com/forums/283924-entity-framework-plus
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class QueryInclude_Include
    {
        [TestMethod]
        public void Many_ThenQuery_Executor()
        {
            TestContext.DeleteAll(x => x.Association_Multi_OneToMany_Lefts);
            TestContext.DeleteAll(x => x.Association_Multi_OneToMany_Right1s);
            TestContext.DeleteAll(x => x.Association_Multi_OneToMany_Right2s);

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
                    .Include(left => left.Right1s, true)
                    .Include(left => left.Right2s, true)
                    .Where(x => x.ColumnInt < 5)
                    .OrderBy(x => x.ID)
                    .First();

                // TEST: context
                Assert.AreEqual(11, ctx.ChangeTracker.Entries().Count());
            }
        }
    }
}