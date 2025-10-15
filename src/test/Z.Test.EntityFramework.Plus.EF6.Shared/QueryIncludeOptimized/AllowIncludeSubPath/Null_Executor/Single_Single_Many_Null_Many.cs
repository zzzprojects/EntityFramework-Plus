// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class QueryIncludeOptimized_AllowIncludeSubPath_Null_Executor
    {
        [TestMethod]
        public void Single_Single_Many_Null_Many()
        {
            try
            {
                QueryIncludeOptimizedManager.AllowIncludeSubPath = true;
                QueryIncludeOptimizedHelper.InsertOneToOneAndMany(single1: true, single2: true, many3: false);

                using (var ctx = new TestContext())
                {
                    var left = ctx.Association_OneToSingleAndMany_Lefts
                        .IncludeOptimized(x => x.Single_Right.Single_RightRight.Many_RightRightRight.Select(y => y.Many_RightRightRightRight))
                        .First();

                    var list1 = new List<Association_OneToSingleAndMany_Right>();
                    var list2 = new List<Association_OneToSingleAndMany_RightRight>();
                    var list3 = new List<Association_OneToSingleAndMany_RightRightRight>();

                    // Level 1
                    {
                        Assert.IsNotNull(left.Single_Right);
                        Assert.IsNull(left.Many_Right);

                        list1.Add(left.Single_Right);
                    }

                    // Level 2
                    {
                        foreach (var item in list1)
                        {
                            Assert.IsNotNull(item.Single_RightRight);
                            Assert.IsNull(item.Many_RightRight);

                            list2.Add(item.Single_RightRight);
                        }
                    }

                    // Level 3
                    {
                        foreach (var item in list2)
                        {
                            Assert.IsNull(item.Single_RightRightRight);
                            Assert.AreEqual(0, item.Many_RightRightRight.Count);
                        }
                    }
                }
            }
            finally
            {
                QueryIncludeOptimizedManager.AllowIncludeSubPath = false;
            }
        }
    }
}