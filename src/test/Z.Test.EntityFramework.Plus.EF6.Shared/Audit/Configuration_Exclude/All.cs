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
    public partial class Audit_Configuration_Exclude
    {
        [TestMethod]
        public void All()
        {
            var identitySeedCat = TestContext.GetIdentitySeed(x => x.Inheritance_TPC_Animals, Inheritance_TPC_Cat.Create);
            var identitySeedDog = TestContext.GetIdentitySeed(x => x.Inheritance_TPC_Animals, Inheritance_TPC_Dog.Create);

            TestContext.DeleteAll(x => x.AuditEntryProperties);
            TestContext.DeleteAll(x => x.AuditEntries);
            TestContext.DeleteAll(x => x.Inheritance_TPC_Animals);

            var audit = AuditHelper.AutoSaveAudit();
            audit.Configuration.Exclude(x => true);

            using (var ctx = new TestContext())
            {
                TestContext.Insert(ctx, x => x.Inheritance_TPC_Animals, Inheritance_TPC_Cat.Create, 1);
                TestContext.Insert(ctx, x => x.Inheritance_TPC_Animals, Inheritance_TPC_Dog.Create, 2);
                ctx.SaveChanges(audit);
            }

            // UnitTest - Audit
            {
                var entries = audit.Entries;

                // Entries
                {
                    // Entries Count
                    Assert.AreEqual(0, entries.Count);
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
                        Assert.AreEqual(0, entries.Count);
                    }
                }
            }
        }
    }
}

#endif