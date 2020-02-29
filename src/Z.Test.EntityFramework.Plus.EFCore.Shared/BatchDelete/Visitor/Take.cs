// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class BatchDelete_Visitor
    {
        [TestMethod]
        public void Take()
        {
            TestContext.DeleteAll(x => x.Entity_Basics);
            TestContext.Insert(x => x.Entity_Basics, 50);

            using (var ctx = new TestContext())
            {
                var sql = "";

                // BEFORE
                Assert.AreEqual(1225, ctx.Entity_Basics.Sum(x => x.ColumnInt));
                
                //for keep in mind that don't work
                bool notwork = false;

                try
                { 

                // not work in Core.
                // var list = ctx.Entity_Basics.Where(x => x.ColumnInt > 10 && x.ColumnInt <= 40).OrderBy(x => x.ColumnInt).Take(20).Take(10).Select(x => x.ID).ToList();
                // ACTION
                var rowsAffected = ctx.Entity_Basics.Where(x => x.ColumnInt > 10 && x.ColumnInt <= 40).OrderBy(x => x.ColumnInt).Take(20).Delete(delete => delete.Executing = command => sql = command.CommandText);

                // AFTER
                Assert.AreEqual(815, ctx.Entity_Basics.Sum(x => x.ColumnInt));
                Assert.AreEqual(20, rowsAffected);

#if EF5
                Assert.AreEqual(@"
DECLARE @stop int
DECLARE @rowAffected INT
DECLARE @totalRowAffected INT

SET @stop = 0
SET @totalRowAffected = 0

WHILE @stop=0
    BEGIN
        DELETE TOP (4000)
        FROM    A
        FROM    [dbo].[Entity_Basic] AS A
                INNER JOIN ( SELECT 
[Extent1].[ID] AS [ID]
FROM [dbo].[Entity_Basic] AS [Extent1]
WHERE ([Extent1].[ColumnInt] > 10) AND ([Extent1].[ColumnInt] <= 40)
                           ) AS B ON A.[ID] = B.[ID]

        SET @rowAffected = @@ROWCOUNT
        SET @totalRowAffected = @totalRowAffected + @rowAffected

		IF @rowAffected < 4000
            SET @stop = 1
    END

SELECT  @totalRowAffected
", sql);
#elif EF6
                  Assert.AreEqual(@"
DELETE
FROM    A
FROM    [dbo].[Entity_Basic] AS A
        INNER JOIN ( SELECT TOP (20) 
    [Extent1].[ID] AS [ID]
    FROM [dbo].[Entity_Basic] AS [Extent1]
    WHERE ([Extent1].[ColumnInt] > 10) AND ([Extent1].[ColumnInt] <= 40)
    ORDER BY [Extent1].[ID] ASC
                    ) AS B ON A.[ID] = B.[ID]

SELECT @@ROWCOUNT
", sql);
#elif EFCORE
                    Assert.AreEqual(@"
DELETE
FROM    A
FROM    [Entity_Basic] AS A
        INNER JOIN ( SELECT [t1].[ID]
FROM (
    SELECT TOP(@__p_1) [t0].*
    FROM (
        SELECT TOP(@__p_0) [x0].*
        FROM [Entity_Basic] AS [x0]
        WHERE ([x0].[ColumnInt] > 10) AND ([x0].[ColumnInt] <= 40)
        ORDER BY [x0].[ColumnInt]
    ) AS [t0]
) AS [t1]
                    ) AS B ON A.[ID] = B.[ID]

SELECT @@ROWCOUNT
", sql);
#endif
                }
                catch
                {
                    notwork = true;
                }

                Assert.IsTrue(notwork);

            }
        }
    }
}