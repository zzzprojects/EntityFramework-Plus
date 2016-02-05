// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2016 ZZZ Projects. All rights reserved.

#if EF6
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class QueryIncludeOptimized_Where_Async
    {
        [TestMethod]
        public async Task Many_EnumeratorAsync()
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
                var list = await ctx.Association_Multi_OneToMany_Lefts
                    .IncludeOptimized(left => left.Right1s.Where(y => y.ColumnInt > 2))
                    .IncludeOptimized(left => left.Right2s.Where(y => y.ColumnInt > 2))
                    .ToListAsync();

                // TEST: context
                Assert.AreEqual(5, ctx.ChangeTracker.Entries().Count());

                // TEST: left
                Assert.AreEqual(1, list.Count);
                var item = list[0];

                // TEST: right1
                {
                    Assert.AreEqual(2, item.Right1s.Count);
                    var first = item.Right1s[0].ColumnInt == 3 ? item.Right1s[0] : item.Right1s[1];
                    var second = item.Right1s[0].ColumnInt == 3 ? item.Right1s[1] : item.Right1s[0];
                    Assert.AreEqual(3, first.ColumnInt);
                    Assert.AreEqual(4, second.ColumnInt);
                }

                // TEST: right2
                {
                    Assert.AreEqual(2, item.Right2s.Count);
                    var first = item.Right2s[0].ColumnInt == 3 ? item.Right2s[0] : item.Right2s[1];
                    var second = item.Right2s[0].ColumnInt == 3 ? item.Right2s[1] : item.Right2s[0];
                    Assert.AreEqual(3, first.ColumnInt);
                    Assert.AreEqual(4, second.ColumnInt);
                }
            }
        }
    }
}

#endif