// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public static void InsertFactory<T>(T item, int i)
        {
            if (typeof (T) == typeof (Entity_Complex))
            {
                var complex = (Entity_Complex) (object) item;
                complex.ColumnInt = i;
                complex.Info = new Entity_Complex_Info();
                complex.Info.ColumnInt = 1000 + i;
                complex.Info.Info = new Entity_Complex_Info_Info();
                complex.Info.Info.ColumnInt = 1000000 + i;
            }
            else
            {
                PropertyInfo property;

                property = item.GetType().GetProperty("ColumnInt", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (property != null)
                {
                    property.SetValue(item, i);
                }

                property = item.GetType().GetProperty("Column1", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (property != null)
                {
                    property.SetValue(item, i);
                }

                property = item.GetType().GetProperty("Column2", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (property != null)
                {
                    property.SetValue(item, i);
                }

                property = item.GetType().GetProperty("Column3", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (property != null)
                {
                    property.SetValue(item, i);
                }

                property = item.GetType().GetProperty("ColumnCat");
                if (property != null)
                {
                    property.SetValue(item, i);
                }

                property = item.GetType().GetProperty("ColumnDog");
                if (property != null)
                {
                    property.SetValue(item, i);
                }
            }
        }

        public static List<T> Insert<T>(TestContext ctx, Func<TestContext, DbSet<T>> func, int count) where T : class, new()
        {
            var sets = func(ctx);

            var list = new List<T>();
            for (var i = 0; i < count; i++)
            {
                var item = new T();
                InsertFactory(item, i);
                list.Add(item);
            }

            sets.AddRange(list);

            return list;
        }

        public static List<T> Insert<T, T2>(TestContext ctx, Func<TestContext, DbSet<T>> func, Func<T2> factory, int count) where T : class where T2 : T
        {
            var sets = func(ctx);

            var list = new List<T>();
            for (var i = 0; i < count; i++)
            {
                var item = factory();
                InsertFactory(item, i);
                list.Add(item);
            }

            sets.AddRange(list);

            return list;
        }

        public static List<T> Insert<T>(Func<TestContext, DbSet<T>> func, int count) where T : class, new()
        {
            var ctx = new TestContext();
            var sets = func(ctx);


            var countBefore = sets.Count();

            var list = new List<T>();
            for (var i = 0; i < count; i++)
            {
                var item = new T();
                InsertFactory(item, i);
                list.Add(item);
            }

            sets.AddRange(list);
            ctx.SaveChanges();

            Assert.AreEqual(count + countBefore, sets.Count());

            return list;
        }

        public static List<T> Insert<T, T2>(Func<TestContext, DbSet<T>> func, Func<T2> factory, int count) where T : class where T2 : T
        {
            var ctx = new TestContext();
            var sets = func(ctx);

            var countBefore = sets.Count();

            var list = new List<T>();
            for (var i = 0; i < count; i++)
            {
                var item = factory();
                InsertFactory(item, i);
                list.Add(item);
            }

            sets.AddRange(list);
            ctx.SaveChanges();

            Assert.AreEqual(count + countBefore, sets.Count());

            return list;
        }
    }
}