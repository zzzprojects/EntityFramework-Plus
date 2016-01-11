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
using System.Data.Entity;

#elif EF7
using Microsoft.Data.Entity;

#endif

namespace Z.Test.EntityFramework.Plus
{
    public partial class Audit_Configuration
    {
        [TestMethod]
        public void IncludePropertyUnchanged_IsFalse()
        {
            TestContext.DeleteAll(x => x.AuditEntries);
            TestContext.DeleteAll(x => x.Entity_Complexes);

            TestContext.Insert(x => x.Entity_Complexes, 1);

            var audit = AuditHelper.AutoSaveAudit();
            Assert.AreEqual(false, audit.Configuration.IncludePropertyUnchanged);

            using (var ctx = new TestContext())
            {
                ctx.Entity_Complexes.ToList().ForEach(x => x.Info.Info.ColumnInt++);
                ctx.SaveChanges(audit);
            }

            // UnitTest - Audit
            {
                var entries = audit.Entries;

                // Entries
                {
                    // Entries Count
                    Assert.AreEqual(1, entries.Count);

                    // Entries State
                    Assert.AreEqual(AuditEntryState.EntityModified, entries[0].State);
                }

                // Properties
                {
                    var propertyIndex = -1;

                    // Properties Count
                    Assert.AreEqual(1, entries[0].Properties.Count);

                    // Info.Info.ColumnInt
                    propertyIndex = 0;
                    Assert.AreEqual("Info.Info.ColumnInt", entries[0].Properties[propertyIndex].PropertyName);
                    Assert.AreEqual(1000000, entries[0].Properties[propertyIndex].OldValue);
                    Assert.AreEqual(1000001, entries[0].Properties[propertyIndex].NewValue);
                }
            }

            // UnitTest - Audit (Database)
            {
                using (var ctx = new TestContext())
                {
                    // ENSURE order
                    var entries = ctx.AuditEntries.OrderBy(x => x.AuditEntryID).Include(x => x.Properties).ToList();
                    entries.ForEach(x => x.Properties = x.Properties.OrderBy(y => y.AuditEntryPropertyID).ToList());

                    // Entries
                    {
                        var propertyIndex = -1;

                        // Properties Count
                        Assert.AreEqual(1, entries[0].Properties.Count);

                        // Info.Info.ColumnInt
                        propertyIndex = 0;
                        Assert.AreEqual("Info.Info.ColumnInt", entries[0].Properties[propertyIndex].PropertyName);
                        Assert.AreEqual("1000000", entries[0].Properties[propertyIndex].OldValue);
                        Assert.AreEqual("1000001", entries[0].Properties[propertyIndex].NewValue);
                    }
                }
            }
        }
    }
}

#endif