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
        public void WithGlobalFilter_ManyFilter_Enabled()
        {
            using (var ctx = new TestContext(false, enableFilter1: true, enableFilter2: true, enableFilter3: true, enableFilter4: true))
            {
                Assert.AreEqual(35, ctx.Inheritance_Interface_Entities.Filter(
                    QueryFilterHelper.Filter.Filter1,
                    QueryFilterHelper.Filter.Filter2,
                    QueryFilterHelper.Filter.Filter3,
                    QueryFilterHelper.Filter.Filter4).Sum(x => x.ColumnInt));
            }
        }
    }
}