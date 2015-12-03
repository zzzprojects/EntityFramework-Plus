// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: http://zzzprojects.uservoice.com/forums/283924-entity-framework-plus
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

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
            using (var ctx = new EntityContext())
            {
                ctx.EntitySimples.Add(new EntitySimple {ColumnInt = 1});
                ctx.SaveChanges();
                Assert.AreEqual(1, ctx.EntitySimples.ToList().Count);
            }
        }
    }
}