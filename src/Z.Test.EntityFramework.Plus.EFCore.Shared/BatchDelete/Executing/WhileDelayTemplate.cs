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
    public partial class BatchDelete_Executing
    {
        [TestMethod]
        public void WhileDelayTemplate()
        {
            TestContext.DeleteAll(x => x.Entity_Basics);
            TestContext.Insert(x => x.Entity_Basics, 50);

            using (var ctx = new TestContext())
            {
                var sql = "";

                // BEFORE
                Assert.AreEqual(1225, ctx.Entity_Basics.Sum(x => x.ColumnInt));

                // ACTION
                var rowsAffected = ctx.Entity_Basics.Where(x => x.ColumnInt > 10 && x.ColumnInt <= 40).Delete(delete =>
                {
                    delete.BatchDelayInterval = 50;
                    delete.Executing = command => sql = command.CommandText;
                });

                // AFTER
                Assert.AreEqual(460, ctx.Entity_Basics.Sum(x => x.ColumnInt));
                Assert.AreEqual(30, rowsAffected);

#if EF5
                Assert.AreEqual(@"
DECLARE @stop int
DECLARE @rowAffected INT
DECLARE @totalRowAffected INT

SET @stop = 0
SET @totalRowAffected = 0

WHILE @stop=0
    BEGIN
        IF @rowAffected IS NOT NULL
            BEGIN
                WAITFOR DELAY '00:00:00:050'
            END

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
DECLARE @stop int
DECLARE @rowAffected INT
DECLARE @totalRowAffected INT

SET @stop = 0
SET @totalRowAffected = 0

WHILE @stop=0
    BEGIN
        IF @rowAffected IS NOT NULL
            BEGIN
                WAITFOR DELAY '00:00:00:050'
            END

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
#elif EFCORE_2X
                Assert.AreEqual(@"
DECLARE @stop int
DECLARE @rowAffected INT
DECLARE @totalRowAffected INT

SET @stop = 0
SET @totalRowAffected = 0

WHILE @stop=0
    BEGIN
        IF @rowAffected IS NOT NULL
            BEGIN
                WAITFOR DELAY '00:00:00:050'
            END

        DELETE TOP (4000)
        FROM    A 
        FROM    [Entity_Basic] AS A
                INNER JOIN ( SELECT [x].[ID]
FROM [Entity_Basic] AS [x]
WHERE ([x].[ColumnInt] > 10) AND ([x].[ColumnInt] <= 40)
                           ) AS B ON A.[ID] = B.[ID]

        SET @rowAffected = @@ROWCOUNT
        SET @totalRowAffected = @totalRowAffected + @rowAffected

        IF @rowAffected < 4000
            SET @stop = 1
    END

SELECT  @totalRowAffected
", sql);
#elif EFCORE_3X
                Assert.AreEqual(@"
DECLARE @stop int
DECLARE @rowAffected INT
DECLARE @totalRowAffected INT

SET @stop = 0
SET @totalRowAffected = 0

WHILE @stop=0
    BEGIN
        IF @rowAffected IS NOT NULL
            BEGIN
                WAITFOR DELAY '00:00:00:050'
            END

        DELETE TOP (4000)
        FROM    A 
        FROM    [Entity_Basic] AS A
                INNER JOIN ( SELECT [e].[ID]
FROM [Entity_Basic] AS [e]
WHERE ([e].[ColumnInt] > 10) AND ([e].[ColumnInt] <= 40)
                           ) AS B ON A.[ID] = B.[ID]

        SET @rowAffected = @@ROWCOUNT
        SET @totalRowAffected = @totalRowAffected + @rowAffected

        IF @rowAffected < 4000
            SET @stop = 1
    END

SELECT  @totalRowAffected
", sql);
#endif
            }
        }
    }
}