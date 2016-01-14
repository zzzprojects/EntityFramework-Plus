// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

#if EF5 || EF6
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

#if EF5 || EF6

#elif EF7
using Microsoft.Data.Entity;

#endif

namespace Z.Test.EntityFramework.Plus
{
    public partial class Audit_Configuration
    {
        [TestMethod]
        public void IncludeEntityModified_IsFalse()
        {
            TestContext.DeleteAll(x => x.AuditEntries);
            TestContext.DeleteAll(x => x.Entity_Basics);

            TestContext.Insert(x => x.Entity_Basics, 2);

            var audit = AuditHelper.AutoSaveAudit();
            audit.Configuration.IncludeEntityModified = false;

            using (var ctx = new TestContext())
            {
                var list = ctx.Entity_Basics.OrderBy(x => x.ID).ToList();

                // INSERT one
                TestContext.Insert(ctx, x => x.Entity_Basics, 1);

                // DELETE one
                ctx.Entity_Basics.Remove(list[0]);

                // UPDATE one
                list[1].ColumnInt++;

                ctx.SaveChanges(audit);
            }

            // UnitTest - Audit
            {
                var entries = audit.Entries;

                // Entries
                {
                    // Entries Count
                    Assert.AreEqual(2, entries.Count);

                    // Entries State
                    Assert.AreEqual(AuditEntryState.EntityAdded, entries[0].State);
                    Assert.AreEqual(AuditEntryState.EntityDeleted, entries[1].State);
                }
            }

            // UnitTest - Audit (Database)
            {
                using (var ctx = new TestContext())
                {
                    // ENSURE order
                    var entries = ctx.AuditEntries.OrderBy(x => x.AuditEntryID).ToList();

                    // Entries
                    {
                        // Entries Count
                        Assert.AreEqual(2, entries.Count);

                        // Entries State
                        Assert.AreEqual(AuditEntryState.EntityAdded, entries[0].State);
                        Assert.AreEqual(AuditEntryState.EntityDeleted, entries[1].State);
                    }
                }
            }
        }
    }
}

#endif