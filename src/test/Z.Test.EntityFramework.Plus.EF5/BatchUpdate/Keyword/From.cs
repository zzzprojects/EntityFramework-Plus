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
    public partial class BatchUpdate_Keyword
    {
        [TestMethod]
        public void From()
        {
            TestContext.DeleteAll(x => x.Property_AllTypes);
            TestContext.Insert(x => x.Property_AllTypes, 50);

            using (var ctx = new TestContext())
            {
                // BEFORE
                Assert.AreEqual(1225, ctx.Property_AllTypes.Sum(x => x.ColumnInt));

                
                // ACTION
                var rowsAffected = ctx.Property_AllTypes.Where(x => x.ColumnInt > 10 && x.ColumnInt <= 20).Update(x => new Property_AllType { ColumnInt = 99, VarcharColumn = x.VarcharColumn + " from ZZZ Projects"});

                // AFTER
                Assert.AreEqual(2060, ctx.Property_AllTypes.Sum(x => x.ColumnInt));
                Assert.AreEqual(10, rowsAffected);
                // update A  colum = Null + any   ==> colum == Null  in EF6 they have a case null, not in core.
                // Assert.AreEqual(10, ctx.Property_AllTypes.Count(x => !string.IsNullOrEmpty(x.VarcharColumn)));
                Assert.AreNotEqual(10, ctx.Property_AllTypes.Count(x => !string.IsNullOrEmpty(x.VarcharColumn)));
                ctx.Property_AllTypes.Where(x => !string.IsNullOrEmpty(x.VarcharColumn)).ToList().ForEach(x =>
                {
                    Assert.AreEqual(" from ZZZ Projects", x.VarcharColumn);
                }); 
            }
        }
    }
}