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
    public partial class QueryFuture_Future
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

                var futureList1 = ctx.EntitySimples.Where(x => x.ColumnInt < 5).Future();
                var futureList2 = ctx.EntitySimples.Where(x => x.ColumnInt >= 5).Future();

                // TEST: The cache count are NOT equal (A new context has been added)
                Assert.AreEqual(cacheCountBefore + 1, QueryFutureManager.Cache.Count());

                var list = futureList1.ToList();

                // AFTER

                // TEST: The cache count are equal (The new context has been removed)
                Assert.AreEqual(cacheCountBefore, QueryFutureManager.Cache.Count());

                // TEST: The futureList1 has a value and the list contains 5 items
                Assert.IsTrue(futureList1.HasValue);
                Assert.AreEqual(5, futureList1.ToList().Count);
                Assert.AreEqual(5, list.Count);

                // TEST: The futureList2 has a value and the list contains 5 items
                Assert.IsTrue(futureList2.HasValue);
                Assert.AreEqual(5, futureList2.ToList().Count);

                // TEST: All entries has been loaded in the change tracker
                Assert.AreEqual(10, ctx.ChangeTracker.Entries().Count());
            }
        }
    }
}