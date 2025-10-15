// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if EF5 || EF6
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;
#if EF5 || EF6

#elif EFCORE
using Microsoft.Data.Entity;

#endif

namespace Z.Test.EntityFramework.Plus
{
    public partial class Audit_GetAudits_ByKey
    {
        [TestMethod]
        public void Array_Less()
        {
            TestContext.DeleteAll(x => x.AuditEntryProperties);
            TestContext.DeleteAll(x => x.AuditEntries);
            TestContext.DeleteAll(x => x.Entity_Basics);

            TestContext.Insert(x => x.Entity_Basics, 3);

            var audit = AuditHelper.AutoSaveAudit();

            Entity_Basic auditedItem = null;

            using (var ctx = new TestContext())
            {
                auditedItem = TestContext.Insert(ctx, x => x.Entity_Basics, 3)[2];
                ctx.SaveChanges(audit);
            }

            // UnitTest - Audit
            {
                using (var ctx = new TestContext())
                {
                    try
                    {
                        var auditEntries = ctx.AuditEntries.Where<Entity_Basic>().ToList();

                        throw new Exception("Oops! Invalid exception message.");
                    }
                    catch (Exception ex)
                    {
                        Assert.AreEqual(ExceptionMessage.Audit_Key_OutOfBound, ex.Message);
                    }
                }
            }
        }
    }
}

#endif