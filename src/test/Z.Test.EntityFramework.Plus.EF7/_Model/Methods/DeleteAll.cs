// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if EF5 || EF6
using System.Data.Entity;

#elif EF7
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