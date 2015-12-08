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
    public partial class QueryFuture_FutureValue
    {
        [TestMethod]
        public void Queryable_AsTracking()
        {
            EntitySimpleHelper.Clear();
            EntitySimpleHelper.AddTen();

            using (var ctx = new EntityContext())
            {
                // BEFORE
                var cacheCountBefore = QueryFutureManager.Cache.Count();

                var futureValue1 = ctx.EntitySimples.Where(x => x.ColumnInt < 5).OrderBy(x => x.ColumnInt).FutureValue();
                var futureValue2 = ctx.EntitySimples.Where(x => x.ColumnInt >= 5).OrderBy(x => x.ColumnInt).FutureValue();

                // TEST: The cache count are NOT equal (A new context has been added)
                Assert.AreEqual(cacheCountBefore + 1, QueryFutureManager.Cache.Count());

                var value = futureValue1.Value;

                // AFTER

                // TEST: The cache count are equal (The new context has been removed)
                Assert.AreEqual(cacheCountBefore, QueryFutureManager.Cache.Count());

                // TEST: The futureList1 has a value and the first item is returned
                Assert.IsTrue(futureValue1.HasValue);
                Assert.AreEqual(0, value.ColumnInt);

                // TEST: The futureList2 has a value and the first item is returned
                Assert.IsTrue(futureValue2.HasValue);
                Assert.AreEqual(5, futureValue2.Value.ColumnInt);

                // TEST: The first item of both FutureValue has been added
                Assert.AreEqual(2, ctx.ChangeTracker.Entries().Count());
            }
        }
    }
}