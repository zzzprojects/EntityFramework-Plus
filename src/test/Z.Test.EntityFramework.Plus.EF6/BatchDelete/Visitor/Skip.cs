// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class BatchDelete_Visitor
    {
        [TestMethod]
        public void Skip()
        {
            Action action = () =>
            {
                TestContext.DeleteAll(x => x.Entity_Basics);
                TestContext.Insert(x => x.Entity_Basics, 50);

                using (var ctx = new TestContext())
                {
                    var sql = "";

                    // BEFORE
                    Assert.AreEqual(1225, ctx.Entity_Basics.Sum(x => x.ColumnInt));

                    // ACTION
                    var rowsAffected = ctx.Entity_Basics.Where(x => x.ColumnInt > 10 && x.ColumnInt <= 40).OrderBy(x => x.ColumnInt).Skip(20).Delete(delete => delete.Executing = command => sql = command.CommandText);

                    // AFTER
                    Assert.AreEqual(870, ctx.Entity_Basics.Sum(x => x.ColumnInt));
                    Assert.AreEqual(10, rowsAffected);

    #if EF5
                Assert.AreEqual(@"
DELETE
FROM    A
FROM    [dbo].[Entity_Basic] AS A
        INNER JOIN ( SELECT TOP (2147483647) 
[Filter1].[ID] AS [ID]
FROM ( SELECT [Extent1].[ID] AS [ID], row_number() OVER (ORDER BY [Extent1].[ID] ASC) AS [row_number]
	FROM [dbo].[Entity_Basic] AS [Extent1]
	WHERE ([Extent1].[ColumnInt] > 10) AND ([Extent1].[ColumnInt] <= 40)
)  AS [Filter1]
WHERE [Filter1].[row_number] > 20
ORDER BY [Filter1].[ID] ASC
                    ) AS B ON A.[ID] = B.[ID]

SELECT @@ROWCOUNT
", sql);
#elif EF6
                Assert.AreEqual(@"
DELETE
FROM    A 
FROM    [dbo].[Entity_Basic] AS A
        INNER JOIN ( SELECT 
    [Extent1].[ID] AS [ID]
    FROM [dbo].[Entity_Basic] AS [Extent1]
    WHERE ([Extent1].[ColumnInt] > 10) AND ([Extent1].[ColumnInt] <= 40)
    ORDER BY [Extent1].[ColumnInt] ASC
    OFFSET 20 ROWS FETCH NEXT 2147483647 ROWS ONLY 
                    ) AS B ON A.[ID] = B.[ID]

SELECT @@ROWCOUNT
", sql);
#elif EFCORE
                Assert.AreEqual(@"
DELETE
FROM    A
FROM    [Entity_Basic] AS A
        INNER JOIN ( SELECT [t].[ID]
FROM (
    SELECT [x0].*
    FROM [Entity_Basic] AS [x0]
    WHERE ([x0].[ColumnInt] > 10) AND ([x0].[ColumnInt] <= 40)
    ORDER BY [x0].[ColumnInt]
    OFFSET @__p_0 ROWS FETCH NEXT @__p_1 ROWS ONLY
) AS [t]
                    ) AS B ON A.[ID] = B.[ID]

SELECT @@ROWCOUNT
", sql);
#endif
                }
            };

            MyIni.RunWithFailLogical(MyIni.GetSetupCasTest(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name), action);
        }
    }
}