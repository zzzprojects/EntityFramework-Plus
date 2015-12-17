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
    public static class EntitySimpleHelper
    {
        public static void Clear()
        {
            using (var ctx = new EntityContext())
            {
                var list = ctx.EntitySimples.ToList();
                list.ForEach(x => ctx.EntitySimples.Remove(x));
                ctx.SaveChanges();
                Assert.AreEqual(0, ctx.EntitySimples.ToList().Count);
            }
        }

        public static void AddOne()
        {
            Add(1);
        }

        public static void AddTen()
        {
            Add(10);
        }

        public static void Add(int count)
        {
            var entities = new List<EntitySimple>();
            for (var i = 0; i < count; i++)
            {
                entities.Add(new EntitySimple {ColumnInt = i});
            }

            using (var ctx = new EntityContext())
            {
                ctx.EntitySimples.AddRange(entities);
                ctx.SaveChanges();
                Assert.AreEqual(count, ctx.EntitySimples.ToList().Count);
            }
        }
    }
}