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
using System.Data.Entity;

#elif EFCORE
using Microsoft.Data.Entity;

#endif

namespace Z.Test.EntityFramework.Plus
{
    public partial class Audit_Configuration_AuditEntryPropertyFactory
    {
        [TestMethod]
        public void EntitySoftAdded()
        {
            var identitySeed = TestContext.GetIdentitySeed(x => x.Entity_Basics);

            TestContext.DeleteAll(x => x.AuditEntryProperties);
            TestContext.DeleteAll(x => x.AuditEntries);
            TestContext.DeleteAll(x => x.Entity_Basics);

            TestContext.Insert(x => x.Entity_Basics, 3);

            var audit = AuditHelper.AutoSaveWithAuditEntryPropertyFactory();
            audit.Configuration.SoftAdded(x => true);

            using (var ctx = new TestContext())
            {
                ctx.Entity_Basics.ToList().ForEach(x => x.ColumnInt++);

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
                    Assert.AreEqual(AuditEntryState.EntitySoftAdded, entries[0].State);
                    Assert.AreEqual(AuditEntryState.EntitySoftAdded, entries[1].State);
                    Assert.AreEqual(AuditEntryState.EntitySoftAdded, entries[2].State);

                    // Entries EntitySetName
                    Assert.AreEqual(TestContext.TypeName(x => x.Entity_Basics), entries[0].EntitySetName);
                    Assert.AreEqual(TestContext.TypeName(x => x.Entity_Basics), entries[1].EntitySetName);
                    Assert.AreEqual(TestContext.TypeName(x => x.Entity_Basics), entries[2].EntitySetName);

                    // Entries TypeName
                    Assert.AreEqual(typeof (Entity_Basic).Name, entries[0].EntityTypeName);
                    Assert.AreEqual(typeof (Entity_Basic).Name, entries[1].EntityTypeName);
                    Assert.AreEqual(typeof (Entity_Basic).Name, entries[2].EntityTypeName);
                }

                // Properties
                {
                    var propertyIndex = -1;

                    // Properties Count
                    Assert.AreEqual(2, entries[0].Properties.Count);
                    Assert.AreEqual(2, entries[1].Properties.Count);
                    Assert.AreEqual(2, entries[2].Properties.Count);

                    // ID
                    propertyIndex = 0;
                    Assert.AreEqual("CustomPropertyName", entries[0].Properties[propertyIndex].PropertyName);
                    Assert.AreEqual("CustomPropertyName", entries[1].Properties[propertyIndex].PropertyName);
                    Assert.AreEqual("CustomPropertyName", entries[2].Properties[propertyIndex].PropertyName);
                    Assert.AreEqual(identitySeed + 1, entries[0].Properties[propertyIndex].OldValue);
                    Assert.AreEqual(identitySeed + 2, entries[1].Properties[propertyIndex].OldValue);
                    Assert.AreEqual(identitySeed + 3, entries[2].Properties[propertyIndex].OldValue);
                    Assert.AreEqual(identitySeed + 1, entries[0].Properties[propertyIndex].NewValue);
                    Assert.AreEqual(identitySeed + 2, entries[1].Properties[propertyIndex].NewValue);
                    Assert.AreEqual(identitySeed + 3, entries[2].Properties[propertyIndex].NewValue);
                    Assert.AreEqual("CustomExtendedValue", ((AuditEntryProperty_Extended)entries[0].Properties[propertyIndex]).ExtendedValue);
                    Assert.AreEqual("CustomExtendedValue", ((AuditEntryProperty_Extended)entries[1].Properties[propertyIndex]).ExtendedValue);
                    Assert.AreEqual("CustomExtendedValue", ((AuditEntryProperty_Extended)entries[2].Properties[propertyIndex]).ExtendedValue);

                    propertyIndex = 1;
                    Assert.AreEqual("CustomPropertyName", entries[0].Properties[propertyIndex].PropertyName);
                    Assert.AreEqual("CustomPropertyName", entries[1].Properties[propertyIndex].PropertyName);
                    Assert.AreEqual("CustomPropertyName", entries[2].Properties[propertyIndex].PropertyName);
                    Assert.AreEqual(0, entries[0].Properties[propertyIndex].OldValue);
                    Assert.AreEqual(1, entries[1].Properties[propertyIndex].OldValue);
                    Assert.AreEqual(2, entries[2].Properties[propertyIndex].OldValue);
                    Assert.AreEqual(1, entries[0].Properties[propertyIndex].NewValue);
                    Assert.AreEqual(2, entries[1].Properties[propertyIndex].NewValue);
                    Assert.AreEqual(3, entries[2].Properties[propertyIndex].NewValue);
                    Assert.AreEqual("CustomExtendedValue", ((AuditEntryProperty_Extended)entries[0].Properties[propertyIndex]).ExtendedValue);
                    Assert.AreEqual("CustomExtendedValue", ((AuditEntryProperty_Extended)entries[1].Properties[propertyIndex]).ExtendedValue);
                    Assert.AreEqual("CustomExtendedValue", ((AuditEntryProperty_Extended)entries[2].Properties[propertyIndex]).ExtendedValue);
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
                        Assert.AreEqual(3, entries.Count);

                        // Entries State
                        Assert.AreEqual(AuditEntryState.EntitySoftAdded, entries[0].State);
                        Assert.AreEqual(AuditEntryState.EntitySoftAdded, entries[1].State);
                        Assert.AreEqual(AuditEntryState.EntitySoftAdded, entries[2].State);

                        // Entries EntitySetName
                        Assert.AreEqual(TestContext.TypeName(x => x.Entity_Basics), entries[0].EntitySetName);
                        Assert.AreEqual(TestContext.TypeName(x => x.Entity_Basics), entries[1].EntitySetName);
                        Assert.AreEqual(TestContext.TypeName(x => x.Entity_Basics), entries[2].EntitySetName);

                        // Entries TypeName
                        Assert.AreEqual(typeof (Entity_Basic).Name, entries[0].EntityTypeName);
                        Assert.AreEqual(typeof (Entity_Basic).Name, entries[1].EntityTypeName);
                        Assert.AreEqual(typeof (Entity_Basic).Name, entries[2].EntityTypeName);
                    }

                    // Properties
                    {
                        var propertyIndex = -1;

                        // Properties Count
                        Assert.AreEqual(2, entries[0].Properties.Count);
                        Assert.AreEqual(2, entries[1].Properties.Count);
                        Assert.AreEqual(2, entries[2].Properties.Count);

                        // ID
                        propertyIndex = 0;
                        Assert.AreEqual("CustomPropertyName", entries[0].Properties[propertyIndex].PropertyName);
                        Assert.AreEqual("CustomPropertyName", entries[1].Properties[propertyIndex].PropertyName);
                        Assert.AreEqual("CustomPropertyName", entries[2].Properties[propertyIndex].PropertyName);
                        Assert.AreEqual((identitySeed + 1).ToString(), entries[0].Properties[propertyIndex].OldValue);
                        Assert.AreEqual((identitySeed + 2).ToString(), entries[1].Properties[propertyIndex].OldValue);
                        Assert.AreEqual((identitySeed + 3).ToString(), entries[2].Properties[propertyIndex].OldValue);
                        Assert.AreEqual((identitySeed + 1).ToString(), entries[0].Properties[propertyIndex].NewValue);
                        Assert.AreEqual((identitySeed + 2).ToString(), entries[1].Properties[propertyIndex].NewValue);
                        Assert.AreEqual((identitySeed + 3).ToString(), entries[2].Properties[propertyIndex].NewValue);
                        Assert.AreEqual("CustomExtendedValue", ((AuditEntryProperty_Extended)entries[0].Properties[propertyIndex]).ExtendedValue);
                        Assert.AreEqual("CustomExtendedValue", ((AuditEntryProperty_Extended)entries[1].Properties[propertyIndex]).ExtendedValue);
                        Assert.AreEqual("CustomExtendedValue", ((AuditEntryProperty_Extended)entries[2].Properties[propertyIndex]).ExtendedValue);

                        // ColumnInt
                        propertyIndex = 1;
                        Assert.AreEqual("CustomPropertyName", entries[0].Properties[propertyIndex].PropertyName);
                        Assert.AreEqual("CustomPropertyName", entries[1].Properties[propertyIndex].PropertyName);
                        Assert.AreEqual("CustomPropertyName", entries[2].Properties[propertyIndex].PropertyName);
                        Assert.AreEqual("0", entries[0].Properties[propertyIndex].OldValue);
                        Assert.AreEqual("1", entries[1].Properties[propertyIndex].OldValue);
                        Assert.AreEqual("2", entries[2].Properties[propertyIndex].OldValue);
                        Assert.AreEqual("1", entries[0].Properties[propertyIndex].NewValue);
                        Assert.AreEqual("2", entries[1].Properties[propertyIndex].NewValue);
                        Assert.AreEqual("3", entries[2].Properties[propertyIndex].NewValue);
                        Assert.AreEqual("CustomExtendedValue", ((AuditEntryProperty_Extended)entries[0].Properties[propertyIndex]).ExtendedValue);
                        Assert.AreEqual("CustomExtendedValue", ((AuditEntryProperty_Extended)entries[1].Properties[propertyIndex]).ExtendedValue);
                        Assert.AreEqual("CustomExtendedValue", ((AuditEntryProperty_Extended)entries[2].Properties[propertyIndex]).ExtendedValue);
                    }
                }
            }
        }
    }
}

#endif