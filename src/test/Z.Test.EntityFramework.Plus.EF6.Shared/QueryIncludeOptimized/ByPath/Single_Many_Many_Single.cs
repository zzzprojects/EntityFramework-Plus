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
    public partial class QueryIncludeOptimized_ByPath
    {
        [TestMethod]
        public void Single_Many_Many_Single()
        {
            using (var ctx = new TestContext())
            {
                var left = ctx.Association_OneToSingleAndMany_Lefts.IncludeOptimizedByPath("Single_Right.Many_RightRight.Many_RightRightRight.Single_RightRightRightRight").ToList();
            }
        }
    }
}