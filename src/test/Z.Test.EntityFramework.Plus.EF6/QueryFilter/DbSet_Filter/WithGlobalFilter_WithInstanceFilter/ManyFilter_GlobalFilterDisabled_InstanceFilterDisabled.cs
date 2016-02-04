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
    public partial class QueryFilter_DbSet_Filter
    {
        [TestMethod]
        public void WithGlobalFilter_WithInstanceFilter_ManyFilter_GlobalFilterDisabled_InstanceFilterDisabled()
        {
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

                Assert.AreEqual(9, ctx.Inheritance_Interface_Entities.Filter(
                    QueryFilterHelper.Filter.Filter1,
                    QueryFilterHelper.Filter.Filter2,
                    QueryFilterHelper.Filter.Filter3,
                    QueryFilterHelper.Filter.Filter4,
                    QueryFilterHelper.Filter.Filter5,
                    QueryFilterHelper.Filter.Filter6,
                    QueryFilterHelper.Filter.Filter7,
                    QueryFilterHelper.Filter.Filter8).Sum(x => x.ColumnInt));
            }
        }
    }
}