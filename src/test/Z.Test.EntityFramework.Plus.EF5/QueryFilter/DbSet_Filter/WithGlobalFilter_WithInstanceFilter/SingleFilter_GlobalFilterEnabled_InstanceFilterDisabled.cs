// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class QueryFilter_DbSet_Filter
    {
        [TestMethod]
        public void WithGlobalFilter_WithInstanceFilter_SingleFilter_GlobalFilterEnabled_InstanceFilterDisabled()
        {
            using (var ctx = new TestContext(false, enableFilter1: true))
            {
                ctx.Filter<Inheritance_Interface_Entity>(QueryFilterHelper.Filter.Filter5, entities => entities.Where(x => x.ColumnInt != 5));
                ctx.Filter(QueryFilterHelper.Filter.Filter5).Disable();

                Assert.AreEqual(39, ctx.Inheritance_Interface_Entities.Filter(
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