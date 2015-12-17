// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: http://zzzprojects.uservoice.com/forums/283924-entity-framework-plus
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Z.Test.EntityFramework.Plus
{
    public static class FilterEntityHelper
    {
        public enum Filter
        {
            Filter1,
            Filter2,
            Filter3,
            Filter4,
            Filter5,
            Filter6,
            Filter7,
            Filter8
        }

        public static void Clear()
        {
            using (var ctx = new EntityContext())
            {
                var list = ctx.FilterEntities.ToList();

#if EF5
                list.ForEach(x => ctx.FilterEntities.Remove(x));
#elif EF6
                ctx.FilterEntities.RemoveRange(list);
#endif
                ctx.SaveChanges();
                Assert.AreEqual(0, ctx.FilterEntities.ToList().Count);
            }
        }

        public static void AddTen()
        {
            Add(10);
        }

        public static void Add(int count)
        {
            var entities = new List<FilterEntity>();
            for (var i = 0; i < count; i++)
            {
                entities.Add(new FilterEntity {ColumnInt = i});
            }

            using (var ctx = new EntityContext())
            {
#if EF5
                entities.ForEach(x => ctx.FilterEntities.Add(x));
#elif EF6
                ctx.FilterEntities.AddRange(entities);
#endif
                ctx.SaveChanges();
                Assert.AreEqual(count, ctx.FilterEntities.ToList().Count);
            }
        }
    }
}