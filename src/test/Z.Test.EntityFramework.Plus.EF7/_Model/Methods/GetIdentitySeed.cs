// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;
using System.Linq;
#if EF5 || EF6
using System.Data.Entity;

#elif EF7
using Microsoft.Data.Entity;

#endif

namespace Z.Test.EntityFramework.Plus
{
    public partial class TestContext
    {
        public static int GetIdentitySeed<T>(Func<TestContext, DbSet<T>> func) where T : class, new()
        {
            var item = Insert(func, 1).First();

            var property = item.GetType().GetProperty("ID");
            if (property != null)
            {
                var id = property.GetValue(item);
                return Convert.ToInt32(id);
            }

            throw new Exception("Could not found ID property.");
        }

        public static int GetIdentitySeed<T, T2>(Func<TestContext, DbSet<T>> func, Func<T2> factory) where T : class where T2 : T
        {
            var item = Insert(func, factory, 1).First();

            var property = item.GetType().GetProperty("ID");
            if (property != null)
            {
                var id = property.GetValue(item);
                return Convert.ToInt32(id);
            }

            throw new Exception("Could not found ID property.");
        }
    }
}