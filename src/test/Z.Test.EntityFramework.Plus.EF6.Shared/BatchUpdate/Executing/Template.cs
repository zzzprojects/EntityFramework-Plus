// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.


#if TODO
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class BatchUpdate_Executing
    {
        [TestMethod]
        public void Template()
        {
            TestContext.DeleteAll(x => x.Entity_Basics);
            TestContext.Insert(x => x.Entity_Basics, 50);

            using (var ctx = new TestContext())
            {
                var sql = "";

                // BEFORE
                Assert.AreEqual(1225, ctx.Entity_Basics.Sum(x => x.ColumnInt));

                // ACTION
                var rowsAffected = ctx.Entity_Basics.Where(x => x.ColumnInt > 10 && x.ColumnInt <= 40).Update(x => new Entity_Basic { ColumnInt = 99 }, update =>
                {
                    update.BatchSize = 0;
                    update.Executing = command => sql = command.CommandText;
                });

                // AFTER
                Assert.AreEqual(460, ctx.Entity_Basics.Sum(x => x.ColumnInt));
                Assert.AreEqual(30, rowsAffected);
                Assert.AreEqual(@"
DELETE
FROM    A
FROM    [dbo].[Entity_Basic] AS A
        INNER JOIN ( SELECT 
    [Extent1].[ID] AS [ID]
    FROM [dbo].[Entity_Basic] AS [Extent1]
    WHERE ([Extent1].[ColumnInt] > 10) AND ([Extent1].[ColumnInt] <= 40)
                    ) AS B ON A.[ID] = B.[ID]

SELECT @@ROWCOUNT
", sql);
            }
        }
    }
}
#endif