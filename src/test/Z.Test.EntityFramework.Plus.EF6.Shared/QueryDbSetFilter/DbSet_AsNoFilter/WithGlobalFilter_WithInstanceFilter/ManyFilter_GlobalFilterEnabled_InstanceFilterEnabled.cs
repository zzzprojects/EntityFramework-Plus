// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class QueryFilter_DbSet_AsNoDbSetFilter
    {
        [TestMethod]
        public void WithGlobalFilter_WithInstanceFilter_ManyFilter_GlobalFilterEnabled_InstanceFilterEnabled()
        {
            TestContext.DeleteAll(x => x.Inheritance_Interface_Entities);
            TestContext.Insert(x => x.Inheritance_Interface_Entities, 10);

            using (var ctx = new TestContext(true, false, enableFilter1: true, enableFilter2: true, enableFilter3: true, enableFilter4: true))
            {
                ctx.DbSetFilter<Inheritance_Interface_Entity>(QueryFilterHelper.Filter.Filter5, entities => entities.Where(x => x.ColumnInt != 5), false);
                ctx.DbSetFilter<Inheritance_Interface_IEntity>(QueryFilterHelper.Filter.Filter6, entities => entities.Where(x => x.ColumnInt != 6), false);
                ctx.DbSetFilter<Inheritance_Interface_Base>(QueryFilterHelper.Filter.Filter7, entities => entities.Where(x => x.ColumnInt != 7), false);
                ctx.DbSetFilter<Inheritance_Interface_IBase>(QueryFilterHelper.Filter.Filter8, entities => entities.Where(x => x.ColumnInt != 8), false);

                ctx.DbSetFilter(QueryFilterHelper.Filter.Filter5).Enable();
                ctx.DbSetFilter(QueryFilterHelper.Filter.Filter6).Enable();
                ctx.DbSetFilter(QueryFilterHelper.Filter.Filter7).Enable();
                ctx.DbSetFilter(QueryFilterHelper.Filter.Filter8).Enable();

                Assert.AreEqual(45, ctx.Inheritance_Interface_Entities.AsNoDbSetFilter().Sum(x => x.ColumnInt));
            }
        }
    }
}