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
    public partial class QueryFilter_DbContext_DbSetFilter
    {
        [TestMethod]
        public void WithInstanceFilter_ManyFilter_Exclude()
        {
            TestContext.DeleteAll(x => x.Inheritance_Interface_Entities);
            TestContext.Insert(x => x.Inheritance_Interface_Entities, 10);

            using (var ctx = new TestContext())
            {
                ctx.DbSetFilter<Inheritance_Interface_Entity>(QueryFilterHelper.Filter.Filter1, entities => entities.Where(x => x.ColumnInt != 1), false);
                ctx.DbSetFilter<Inheritance_Interface_IEntity>(QueryFilterHelper.Filter.Filter2, entities => entities.Where(x => x.ColumnInt != 2), false);
                ctx.DbSetFilter<Inheritance_Interface_Base>(QueryFilterHelper.Filter.Filter3, entities => entities.Where(x => x.ColumnInt != 3), false);
                ctx.DbSetFilter<Inheritance_Interface_IBase>(QueryFilterHelper.Filter.Filter4, entities => entities.Where(x => x.ColumnInt != 4), false);

                ctx.DbSetFilter(QueryFilterHelper.Filter.Filter1).Enable();
                ctx.DbSetFilter(QueryFilterHelper.Filter.Filter2).Enable();
                ctx.DbSetFilter(QueryFilterHelper.Filter.Filter3).Enable();
                ctx.DbSetFilter(QueryFilterHelper.Filter.Filter4).Enable();

                ctx.DbSetFilter(QueryFilterHelper.Filter.Filter2).Disable(typeof (Inheritance_Interface_IEntity));
                ctx.DbSetFilter(QueryFilterHelper.Filter.Filter3).Disable(typeof (Inheritance_Interface_Base));
                ctx.DbSetFilter(QueryFilterHelper.Filter.Filter4).Disable(typeof (Inheritance_Interface_IBase));

                Assert.AreEqual(44, ctx.Inheritance_Interface_Entities.Sum(x => x.ColumnInt));
            }
        }
    }
}