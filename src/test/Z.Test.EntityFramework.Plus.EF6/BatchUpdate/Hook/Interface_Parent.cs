// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if EF5 || EF6
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;
#if EF5 || EF6
using System.Data.Entity;

#elif EFCORE
using Microsoft.Data.Entity;

#endif

namespace Z.Test.EntityFramework.Plus
{
    public partial class BatchUpdate_Hook
    {
        [TestMethod]
        public void Interface_Parent()
        {
            TestContext.DeleteAll(x => x.Inheritance_Interface_Entities);
            TestContext.Insert(x => x.Inheritance_Interface_Entities, 1);

            using (var ctx = new TestContext(true, true, enableFilter1: true))
            {
                Z.EntityFramework.Extensions.BatchUpdateManager.Hook<Inheritance_Interface_Base>(entity => new Inheritance_Interface_Base() { ColumnInt = 99 });

                ctx.Inheritance_Interface_Entities.Update(x => new Inheritance_Interface_Entity() { ColumnInt2 = -99 });

                Z.EntityFramework.Extensions.BatchUpdateManager.InternalHooks.Clear();

                var item = ctx.Inheritance_Interface_Entities.First();

                Assert.AreEqual(99, item.ColumnInt);
                Assert.AreEqual(-99, item.ColumnInt2);
            }
        }
    }
}

#endif