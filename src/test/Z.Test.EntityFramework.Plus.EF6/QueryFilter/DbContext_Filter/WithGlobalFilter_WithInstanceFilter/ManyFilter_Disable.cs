// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2016 ZZZ Projects. All rights reserved.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class QueryFilter_DbContext_Filter
    {
        [TestMethod]
        public void WithGlobalFilter_WithInstanceFilter_ManyFilte_Disable()
        {
            TestContext.DeleteAll(x => x.Inheritance_Interface_Entities);
            TestContext.Insert(x => x.Inheritance_Interface_Entities, 10);

            using (var ctx = new TestContext(true, enableFilter1: false, enableFilter2: false, enableFilter3: false, enableFilter4: false))
            {
                ctx.Filter<Inheritance_Interface_Entity>(QueryFilterHelper.Filter.Filter5, entities => entities.Where(x => x.ColumnInt != 5));
                ctx.Filter<Inheritance_Interface_IEntity>(QueryFilterHelper.Filter.Filter6, entities => entities.Where(x => x.ColumnInt != 6));
                ctx.Filter<Inheritance_Interface_Base>(QueryFilterHelper.Filter.Filter7, entities => entities.Where(x => x.ColumnInt != 7));
                ctx.Filter<Inheritance_Interface_IBase>(QueryFilterHelper.Filter.Filter8, entities => entities.Where(x => x.ColumnInt != 8));

                ctx.Filter(QueryFilterHelper.Filter.Filter5).Disable();
                ctx.Filter(QueryFilterHelper.Filter.Filter6).Disable();
                ctx.Filter(QueryFilterHelper.Filter.Filter7).Disable();
                ctx.Filter(QueryFilterHelper.Filter.Filter8).Disable();

                Assert.AreEqual(45, ctx.Inheritance_Interface_Entities.Sum(x => x.ColumnInt));
            }
        }
    }
}