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
    public partial class QueryInclude_Where
    {
        [TestMethod]
        public void Many_Executor()
        {
            TestContext.DeleteAll(x => x.Association_Multi_OneToMany_Right1s);
            TestContext.DeleteAll(x => x.Association_Multi_OneToMany_Right2s);
            TestContext.DeleteAll(x => x.Association_Multi_OneToMany_Lefts);

            using (var ctx = new TestContext())
            {
                var left = TestContext.Insert(ctx, x => x.Association_Multi_OneToMany_Lefts, 1).First();
                left.Right1s = TestContext.Insert(ctx, x => x.Association_Multi_OneToMany_Right1s, 5);
                left.Right2s = TestContext.Insert(ctx, x => x.Association_Multi_OneToMany_Right2s, 5);
                ctx.SaveChanges();
            }


            using (var ctx = new TestContext())
            {
                var item = ctx.Association_Multi_OneToMany_Lefts
                    .IncludeQuery(left => left.Right1s.Where(y => y.ColumnInt > 2))
                    .IncludeQuery(left => left.Right2s.Where(y => y.ColumnInt > 2))
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