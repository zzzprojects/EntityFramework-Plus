// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if EF5 || EF6
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

#if EF5 || EF6

#elif EFCORE
using Microsoft.Data.Entity;

#endif

namespace Z.Test.EntityFramework.Plus
{
    public partial class Audit_Configuration_Ignore
    {
        [TestMethod]
        public void IgnoreEntityAdded_IsFalse()
        {
            TestContext.DeleteAll(x => x.AuditEntryProperties);
            TestContext.DeleteAll(x => x.AuditEntries);
            TestContext.DeleteAll(x => x.Entity_Basics);

            TestContext.Insert(x => x.Entity_Basics, 2);

            var audit = AuditHelper.AutoSaveAudit();
            Assert.AreEqual(false, audit.Configuration.IgnoreEntityAdded);

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
                    Assert.AreEqual(3, entries.Count);

                    // Entries State
                    Assert.AreEqual(AuditEntryState.EntityAdded, entries[0].State);
                    Assert.AreEqual(AuditEntryState.EntityModified, entries[1].State);
                    Assert.AreEqual(AuditEntryState.EntityDeleted, entries[2].State);
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
                        Assert.AreEqual(3, entries.Count);

                        // Entries State
                        Assert.AreEqual(AuditEntryState.EntityAdded, entries[0].State);
                        Assert.AreEqual(AuditEntryState.EntityModified, entries[1].State);
                        Assert.AreEqual(AuditEntryState.EntityDeleted, entries[2].State);
                    }
                }
            }
        }
    }
}

#endif