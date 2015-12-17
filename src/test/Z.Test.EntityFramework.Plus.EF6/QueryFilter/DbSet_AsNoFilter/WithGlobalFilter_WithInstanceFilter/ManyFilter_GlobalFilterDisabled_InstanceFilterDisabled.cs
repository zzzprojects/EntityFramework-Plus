// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: http://zzzprojects.uservoice.com/forums/283924-entity-framework-plus
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class QueryFilter_DbSet_AsNoFilter
    {
        [TestMethod]
        public void WithGlobalFilter_WithInstanceFilter_ManyFilter_GlobalFilterDisabled_InstanceFilterDisabled()
        {
            using (var ctx = new EntityContext(true, enableFilter1: false, enableFilter2: false, enableFilter3: false, enableFilter4: false))
            {
                ctx.Filter<FilterEntity>(FilterEntityHelper.Filter.Filter5, entities => entities.Where(x => x.ColumnInt != 5));
                ctx.Filter<IFilterEntity>(FilterEntityHelper.Filter.Filter6, entities => entities.Where(x => x.ColumnInt != 6));
                ctx.Filter<BaseFilterEntity>(FilterEntityHelper.Filter.Filter7, entities => entities.Where(x => x.ColumnInt != 7));
                ctx.Filter<IBaseFilterEntity>(FilterEntityHelper.Filter.Filter8, entities => entities.Where(x => x.ColumnInt != 8));

                ctx.Filter(FilterEntityHelper.Filter.Filter5).Disable();
                ctx.Filter(FilterEntityHelper.Filter.Filter6).Disable();
                ctx.Filter(FilterEntityHelper.Filter.Filter7).Disable();
                ctx.Filter(FilterEntityHelper.Filter.Filter8).Disable();

                Assert.AreEqual(45, ctx.FilterEntities.AsNoFilter().Sum(x => x.ColumnInt));
            }
        }
    }
}