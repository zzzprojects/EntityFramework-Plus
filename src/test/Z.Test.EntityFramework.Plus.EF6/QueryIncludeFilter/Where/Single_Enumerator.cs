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
    public partial class QueryIncludeFilter_Where
    {
        [TestMethod]
        public void Single_Enumerator()
        {
            TestContext.DeleteAll(x => x.Association_OneToMany_Lefts);
            TestContext.DeleteAll(x => x.Association_OneToMany_Rights);

            using (var ctx = new TestContext())
            {
                var left = TestContext.Insert(ctx, x => x.Association_OneToMany_Lefts, 1).First();
                left.Rights = TestContext.Insert(ctx, x => x.Association_OneToMany_Rights, 5);
                ctx.SaveChanges();
            }

            using (var ctx = new TestContext())
            {
                var list = ctx.Association_OneToMany_Lefts
                    .IncludeFilter(left => left.Rights.Where(y => y.ColumnInt > 2))
                    .ToList();

                // TEST: context
                Assert.AreEqual(3, ctx.ChangeTracker.Entries().Count());

                // TEST: left
                Assert.AreEqual(1, list.Count);

                // TEST: right
                var item = list[0];
                Assert.AreEqual(2, item.Rights.Count);
                Assert.AreEqual(3, item.Rights[0].ColumnInt);
                Assert.AreEqual(4, item.Rights[1].ColumnInt);
            }
        }
    }
}