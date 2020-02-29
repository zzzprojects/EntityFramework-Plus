// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;
#if EF5 || EF6
using System.Data.Entity;

#elif EFCORE

#endif

namespace Z.Test.EntityFramework.Plus
{
    public partial class QueryFuture_Custom
    {
        [TestMethod]
        public async Task Queryable_ConcatWithOrderBy_AsNoTracking()
        {
            TestContext.DeleteAll(x => x.Entity_Basics);
            TestContext.Insert(x => x.Entity_Basics, 10);

            TestContext.DeleteAll(x => x.Entity_Guids);
            TestContext.Insert(x => x.Entity_Guids, 10);

            using (var ctx = new TestContext())
            {
                var future1 = ctx.Entity_Basics
                    .Select(eb => eb.ID)
                    .Concat(ctx.Entity_Guids.Select(i => i.ColumnInt).Future())
                    .OrderBy(x => x)
                    .Future();

                var future2 = ctx.Entity_Basics
                    .Select(eb => eb.ID)
                    .Concat(ctx.Entity_Guids.Select(i => i.ColumnInt).Future())
                    .OrderBy(x => x)
                    .Future();

                // DOESN'T WORK with ToListAsync()... due to having 2 queries... we don't care, use the non async
                var noFutureResult = await ctx.Entity_Basics
                    .Select(eb => eb.ID)
                    .Concat(ctx.Entity_Guids.Select(i => i.ColumnInt).Future())
                    .OrderBy(x => x)
                    .ToListAsync();

                Assert.AreEqual(2, QueryFutureManager.AddOrGetBatch(ctx.GetObjectContext()).Queries.Count);

                var futureResult1 = await future1.ToListAsync();
                var futureResult2 = await future2.ToListAsync();

                Assert.AreEqual(0, QueryFutureManager.AddOrGetBatch(ctx.GetObjectContext()).Queries.Count);

                Assert.IsTrue(future1.HasValue);
                Assert.IsTrue(future2.HasValue);

                CollectionAssert.AreEqual(noFutureResult, futureResult1);
                CollectionAssert.AreEqual(noFutureResult, futureResult2);
            }
        }
    }
}