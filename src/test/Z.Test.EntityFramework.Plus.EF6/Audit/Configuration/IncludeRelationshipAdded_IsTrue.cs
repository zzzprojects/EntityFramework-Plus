// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

#if EF5 || EF6
using System.Collections.Generic;
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
        public void IncludeRelationshipAdded_IsTrue()
        {
            TestContext.DeleteAll(x => x.AuditEntries);
            TestContext.DeleteAll(x => x.Association_OneToMany_Lefts);
            TestContext.DeleteAll(x => x.Association_OneToMany_Rights);

            var audit = AuditHelper.AutoSaveAudit();
            Assert.AreEqual(true, audit.Configuration.IncludeRelationshipAdded);

            using (var ctx = new TestContext())
            {
                var left = TestContext.Insert(ctx, x => x.Association_OneToMany_Lefts, 2).First();

                var right = new Association_OneToMany_Right {ColumnInt = 0};

                left.Rights = new List<Association_OneToMany_Right> {right};
                ctx.SaveChanges();
            }

            using (var ctx = new TestContext())
            {
                var list = ctx.Association_OneToMany_Lefts.OrderBy(x => x.ID).ToList();
                var right = ctx.Association_OneToMany_Rights.First();

                // DELETE one
                list[0].Rights.Clear();

                // INSERT one
                list[1].Rights = new List<Association_OneToMany_Right> {right};

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
                    Assert.AreEqual(AuditEntryState.RelationshipAdded, entries[0].State);
                    Assert.AreEqual(AuditEntryState.RelationshipDeleted, entries[1].State);
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
                        Assert.AreEqual(AuditEntryState.RelationshipAdded, entries[0].State);
                        Assert.AreEqual(AuditEntryState.RelationshipDeleted, entries[1].State);
                    }
                }
            }
        }
    }
}

#endif