// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if EF5
using System.Data.Entity;
using Z.EntityFramework.Plus;

#elif EF6
using System.Data.Entity;

#elif EFCORE
using Microsoft.Data.Entity;

#endif

namespace Z.Test.EntityFramework.Plus
{
    public partial class TestContext
    {
        public static void DeleteAll<T>(TestContext ctx, Func<TestContext, DbSet<T>> func) where T : class
        {
            var sets = func(ctx);
            sets.RemoveRange(sets);
        }

        public static void DeleteAll<T>(Func<TestContext, DbSet<T>> func) where T : class
        {
            var ctx = new TestContext();
            var sets = func(ctx);
            sets.RemoveRange(sets);
            ctx.SaveChanges();

            Assert.AreEqual(0, sets.Count());
        }
    }
}