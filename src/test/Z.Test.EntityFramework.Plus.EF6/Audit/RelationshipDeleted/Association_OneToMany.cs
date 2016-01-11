// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: http://zzzprojects.uservoice.com/forums/283924-entity-framework-plus
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.
#if EF5 || EF6
using System.Collections.Generic;
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
    public partial class Audit_RelationshipDeleted
    {
        [TestMethod]
        public void Association_OneToMany()
        {
            int leftID;
            int rightID_0;
            int rightID_1;

            TestContext.DeleteAll(x => x.AuditEntries);
            TestContext.DeleteAll(x => x.Association_OneToMany_Lefts);
            TestContext.DeleteAll(x => x.Association_OneToMany_Rights);

            var audit = AuditHelper.AutoSaveAudit();

            using (var ctx = new TestContext())
            {
                var left = TestContext.Insert(ctx, x => x.Association_OneToMany_Lefts, 1).First();
                var right0 = new Association_OneToMany_Right() { ColumnInt = 0 };
                var right1 = new Association_OneToMany_Right() { ColumnInt = 1 };

                left.Rights = new List<Association_OneToMany_Right>() { right0, right1 };

                ctx.SaveChanges();

                leftID = left.ID;
                rightID_0 = right0.ID;
                rightID_1 = right1.ID;
            }

            using (var ctx = new TestContext())
            {
                TestContext.DeleteAll(ctx, x => x.Association_OneToMany_Rights);
                ctx.SaveChanges(audit);
            }

            // UnitTest - Audit
            {
                var entries = audit.Entries;

                // Entries
                {
                    // Entries Count
                    Assert.AreEqual(4, entries.Count);

                    // Entries State
                    Assert.AreEqual(AuditEntryState.RelationshipDeleted, entries[0].State);
                    Assert.AreEqual(AuditEntryState.RelationshipDeleted, entries[1].State);

                    // Entries EntitySetName
                    Assert.AreEqual("Association_OneToMany_Left_Rights", entries[0].EntitySetName);
                    Assert.AreEqual("Association_OneToMany_Left_Rights", entries[1].EntitySetName);

                    // Entries TypeName
                    Assert.AreEqual(null, entries[0].TypeName);
                    Assert.AreEqual(null, entries[1].TypeName);
                }

                // Properties
                {
                    var propertyIndex = -1;

                    // Properties Count
                    Assert.AreEqual(2, entries[0].Properties.Count);
                    Assert.AreEqual(2, entries[1].Properties.Count);

                    // Association_OneToMany_Left_Rights_Source;ID
                    propertyIndex = 0;
                    Assert.AreEqual("Association_OneToMany_Left_Rights_Source;ID", entries[0].Properties[propertyIndex].PropertyName);
                    Assert.AreEqual("Association_OneToMany_Left_Rights_Source;ID", entries[1].Properties[propertyIndex].PropertyName);
                    Assert.AreEqual(leftID, entries[0].Properties[propertyIndex].OldValue);
                    Assert.AreEqual(leftID, entries[1].Properties[propertyIndex].OldValue);
                    Assert.AreEqual(null, entries[0].Properties[propertyIndex].NewValue);
                    Assert.AreEqual(null, entries[1].Properties[propertyIndex].NewValue);

                    propertyIndex = 1;
                    Assert.AreEqual("Association_OneToMany_Left_Rights_Target;ID", entries[0].Properties[propertyIndex].PropertyName);
                    Assert.AreEqual("Association_OneToMany_Left_Rights_Target;ID", entries[1].Properties[propertyIndex].PropertyName);
                    Assert.AreEqual(rightID_0, entries[0].Properties[propertyIndex].OldValue);
                    Assert.AreEqual(rightID_1, entries[1].Properties[propertyIndex].OldValue);
                    Assert.AreEqual(null, entries[0].Properties[propertyIndex].NewValue);
                    Assert.AreEqual(null, entries[1].Properties[propertyIndex].NewValue);
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
                        Assert.AreEqual(4, entries.Count);

                        // Entries State
                        Assert.AreEqual(AuditEntryState.RelationshipDeleted, entries[0].State);
                        Assert.AreEqual(AuditEntryState.RelationshipDeleted, entries[1].State);

                        // Entries EntitySetName
                        Assert.AreEqual("Association_OneToMany_Left_Rights", entries[0].EntitySetName);
                        Assert.AreEqual("Association_OneToMany_Left_Rights", entries[1].EntitySetName);

                        // Entries TypeName
                        Assert.AreEqual(null, entries[0].TypeName);
                        Assert.AreEqual(null, entries[1].TypeName);
                    }

                    // Properties
                    {
                        var propertyIndex = -1;

                        // Properties Count
                        Assert.AreEqual(2, entries[0].Properties.Count);
                        Assert.AreEqual(2, entries[1].Properties.Count);

                        // Association_OneToMany_Left_Rights_Source;ID
                        propertyIndex = 0;
                        Assert.AreEqual("Association_OneToMany_Left_Rights_Source;ID", entries[0].Properties[propertyIndex].PropertyName);
                        Assert.AreEqual("Association_OneToMany_Left_Rights_Source;ID", entries[1].Properties[propertyIndex].PropertyName);
                        Assert.AreEqual(leftID.ToString(), entries[0].Properties[propertyIndex].OldValue);
                        Assert.AreEqual(leftID.ToString(), entries[1].Properties[propertyIndex].OldValue);
                        Assert.AreEqual(null, entries[0].Properties[propertyIndex].NewValue);
                        Assert.AreEqual(null, entries[1].Properties[propertyIndex].NewValue);

                        propertyIndex = 1;
                        Assert.AreEqual("Association_OneToMany_Left_Rights_Target;ID", entries[0].Properties[propertyIndex].PropertyName);
                        Assert.AreEqual("Association_OneToMany_Left_Rights_Target;ID", entries[1].Properties[propertyIndex].PropertyName);
                        Assert.AreEqual(rightID_0.ToString(), entries[0].Properties[propertyIndex].OldValue);
                        Assert.AreEqual(rightID_1.ToString(), entries[1].Properties[propertyIndex].OldValue);
                        Assert.AreEqual(null, entries[0].Properties[propertyIndex].NewValue);
                        Assert.AreEqual(null, entries[1].Properties[propertyIndex].NewValue);
                    }
                }
            }
        }
    }
}
#endif