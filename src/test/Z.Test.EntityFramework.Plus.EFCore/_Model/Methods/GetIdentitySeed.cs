// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Linq;
#if EF5 || EF6
using System.Data.Entity;

#elif EFCORE
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
#if EF5
            if (typeof (T2) == typeof (Inheritance_TPC_Dog))
            {
                // todo: Find a better technique
                // Little hack to fix TPC identity issue
                Insert(func, factory, 10);
            }
#endif

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