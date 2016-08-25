// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if EF5 || EF6
using System.Data.Entity;
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
        public void IgnorePropertyUnchanged_IsFalse()
        {
            TestContext.DeleteAll(x => x.AuditEntryProperties);
            TestContext.DeleteAll(x => x.AuditEntries);
            TestContext.DeleteAll(x => x.Entity_Complexes);

            var first = TestContext.Insert(x => x.Entity_Complexes, 1).First();
            var ID = first.ID;

            var audit = AuditHelper.AutoSaveAudit();
            audit.Configuration.IgnorePropertyUnchanged = false;

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
                    Assert.AreEqual(4, entries[0].Properties.Count);

                    // ColumnID
                    propertyIndex = 0;
                    Assert.AreEqual("ID", entries[0].Properties[propertyIndex].PropertyName);
                    Assert.AreEqual(ID, entries[0].Properties[propertyIndex].OldValue);
                    Assert.AreEqual(ID, entries[0].Properties[propertyIndex].NewValue);

                    // ColumnInt
                    propertyIndex = 1;
                    Assert.AreEqual("ColumnInt", entries[0].Properties[propertyIndex].PropertyName);
                    Assert.AreEqual(0, entries[0].Properties[propertyIndex].OldValue);
                    Assert.AreEqual(0, entries[0].Properties[propertyIndex].NewValue);

                    // Info.ColumnInt
                    propertyIndex = 2;
                    Assert.AreEqual("Info.ColumnInt", entries[0].Properties[propertyIndex].PropertyName);
                    Assert.AreEqual(1000, entries[0].Properties[propertyIndex].OldValue);
                    Assert.AreEqual(1000, entries[0].Properties[propertyIndex].NewValue);

                    // Info.Info.ColumnInt
                    propertyIndex = 3;
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
                        // Entries Count
                        Assert.AreEqual(1, entries.Count);

                        // Entries State
                        Assert.AreEqual(AuditEntryState.EntityModified, entries[0].State);
                    }

                    // Properties
                    {
                        var propertyIndex = -1;

                        // Properties Count
                        Assert.AreEqual(4, entries[0].Properties.Count);

                        // ColumnID
                        propertyIndex = 0;
                        Assert.AreEqual("ID", entries[0].Properties[propertyIndex].PropertyName);
                        Assert.AreEqual(ID.ToString(), entries[0].Properties[propertyIndex].OldValue);
                        Assert.AreEqual(ID.ToString(), entries[0].Properties[propertyIndex].NewValue);

                        // ColumnInt
                        propertyIndex = 1;
                        Assert.AreEqual("ColumnInt", entries[0].Properties[propertyIndex].PropertyName);
                        Assert.AreEqual("0", entries[0].Properties[propertyIndex].OldValue);
                        Assert.AreEqual("0", entries[0].Properties[propertyIndex].NewValue);

                        // Info.ColumnInt
                        propertyIndex = 2;
                        Assert.AreEqual("Info.ColumnInt", entries[0].Properties[propertyIndex].PropertyName);
                        Assert.AreEqual("1000", entries[0].Properties[propertyIndex].OldValue);
                        Assert.AreEqual("1000", entries[0].Properties[propertyIndex].NewValue);

                        // Info.Info.ColumnInt
                        propertyIndex = 3;
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