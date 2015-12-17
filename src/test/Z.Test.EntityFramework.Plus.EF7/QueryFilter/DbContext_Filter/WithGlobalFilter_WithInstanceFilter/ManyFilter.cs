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
    public partial class QueryFilter_DbContext_Filter
    {
        [TestMethod]
        public void WithGlobalFilter_WithInstanceFilter_ManyFilter()
        {
            FilterEntityHelper.Clear();
            FilterEntityHelper.AddTen();

            using (var ctx = new EntityContext(true, enableFilter1: true, enableFilter2: true, enableFilter3: true, enableFilter4: true))
            {
                ctx.Filter<FilterEntity>(FilterEntityHelper.Filter.Filter5, entities => entities.Where(x => x.ColumnInt != 5));
                ctx.Filter<IFilterEntity>(FilterEntityHelper.Filter.Filter6, entities => entities.Where(x => x.ColumnInt != 6));
                ctx.Filter<BaseFilterEntity>(FilterEntityHelper.Filter.Filter7, entities => entities.Where(x => x.ColumnInt != 7));
                ctx.Filter<IBaseFilterEntity>(FilterEntityHelper.Filter.Filter8, entities => entities.Where(x => x.ColumnInt != 8));

                Assert.AreEqual(9, ctx.FilterEntities.Sum(x => x.ColumnInt));
            }
        }
    }
}