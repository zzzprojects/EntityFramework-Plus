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
    public partial class BatchUpdate_Value
    {
        [TestMethod]
        public void Many_Arithmetic()
        {
            TestContext.DeleteAll(x => x.Entity_Basic_Manies);
            TestContext.Insert(x => x.Entity_Basic_Manies, 50);

            using (var ctx = new TestContext())
            {
                // BEFORE
                Assert.AreEqual(1225, ctx.Entity_Basic_Manies.Sum(x => x.Column1));
                Assert.AreEqual(1225, ctx.Entity_Basic_Manies.Sum(x => x.Column2));
                Assert.AreEqual(1225, ctx.Entity_Basic_Manies.Sum(x => x.Column3));

                // ACTION
                var rowsAffected = ctx.Entity_Basic_Manies.Where(x => x.Column1 > 10 && x.Column1 <= 40).Update(x => new Entity_Basic_Many { Column1 = 99 + 1 , Column2 = 99 + 2 , Column3 = 99 + 3 });

                // AFTER
                Assert.AreEqual(3460, ctx.Entity_Basic_Manies.Sum(x => x.Column1));
                Assert.AreEqual(3490, ctx.Entity_Basic_Manies.Sum(x => x.Column2));
                Assert.AreEqual(3520, ctx.Entity_Basic_Manies.Sum(x => x.Column3));
                Assert.AreEqual(30, rowsAffected);
            }
        }
    }
}