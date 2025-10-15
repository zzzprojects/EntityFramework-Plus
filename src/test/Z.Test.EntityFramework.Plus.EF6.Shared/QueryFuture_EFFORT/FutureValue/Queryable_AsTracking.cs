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
    public partial class QueryFuture_FutureValue_Effort
    {
        //[TestMethod]
        //public void Queryable_AsTracking()
        //{
        //    // TODO: ZZZ - We have hard time to debug it due to .Value that execute without debug work?

        //    TestContext.DeleteAll(x => x.Entity_Basics, TestContext.EffortConnection);
        //    TestContext.Insert(x => x.Entity_Basics, 10, TestContext.EffortConnection);

        //    using (var ctx = new TestContext(TestContext.EffortConnection))
        //    {
        //        // BEFORE
        //        var futureValue1 = ctx.Entity_Basics.Where(x => x.ColumnInt < 5).OrderBy(x => x.ColumnInt).FutureValue();
        //        var futureValue2 = ctx.Entity_Basics.Where(x => x.ColumnInt >= 5).OrderBy(x => x.ColumnInt).FutureValue();

        //        // TEST: The batch contains 2 queries
        //        Assert.AreEqual(2, QueryFutureManager.AddOrGetBatch(ctx.GetObjectContext()).Queries.Count);

        //        var value = futureValue1.Value;

        //        // AFTER

        //        // TEST: The batch contains 2 queries
        //        Assert.AreEqual(0, QueryFutureManager.AddOrGetBatch(ctx.GetObjectContext()).Queries.Count);

        //        // TEST: The futureList1 has a value and the first item is returned
        //        Assert.IsTrue(futureValue1.HasValue);
        //        Assert.AreEqual(0, value.ColumnInt);

        //        // TEST: The futureList2 has a value and the first item is returned
        //        Assert.IsTrue(futureValue2.HasValue);
        //        Assert.AreEqual(5, futureValue2.Value.ColumnInt);

        //        // TEST: The first item of both FutureValue has been added
        //        Assert.AreEqual(2, ctx.ChangeTracker.Entries().Count());
        //    }
        //}
    }
}