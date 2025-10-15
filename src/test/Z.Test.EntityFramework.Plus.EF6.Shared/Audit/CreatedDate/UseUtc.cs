using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class Audit_CreatedDate
    {
        [TestMethod]
        public void UseUtc()
        {
          var identitySeed = TestContext.GetIdentitySeed(x => x.Entity_Basics);

            TestContext.DeleteAll(x => x.AuditEntryProperties);
            TestContext.DeleteAll(x => x.AuditEntries);
            TestContext.DeleteAll(x => x.Entity_Basics);
            using (var ctx = new TestContext())
            {
                ctx.SaveChanges();
            }

            DateTime dateTimeNow = DateTime.Now;
            DateTime dateUTtcTimeNow = DateTime.UtcNow;

            var audit = AuditHelper.AutoSaveAudit();

            using (var ctx = new TestContext())
            {
                audit.Configuration.UseUtcDateTime = false;
                TestContext.Insert(ctx, x => x.Entity_Basics, 3);
                dateTimeNow = DateTime.Now;
                ctx.SaveChanges(audit);
            }

            {
                var entries = audit.Entries;

                // Entries
                {
                    // Entries Count
                    Assert.AreEqual(3, entries.Count);

                    // Entries State
                    Assert.IsTrue(Math.Abs((entries[0].CreatedDate - dateTimeNow).TotalMinutes) < 1);
                    Assert.IsTrue(Math.Abs((entries[1].CreatedDate - dateTimeNow).TotalMinutes) < 1);
                    Assert.IsTrue(Math.Abs((entries[2].CreatedDate - dateTimeNow).TotalMinutes) < 1);
                }
            }

            using (var ctx = new TestContext())
            {
                audit.Entries.Clear();
                audit.Configuration.UseUtcDateTime = true;
                TestContext.Insert(ctx, x => x.Entity_Basics, 3);
                dateUTtcTimeNow = DateTime.UtcNow;
                ctx.SaveChanges(audit);
            }

            {
                var entries = audit.Entries;

                // Entries
                {
                    // Entries Count
                    Assert.AreEqual(3, entries.Count);

                    // Entries State
                    Assert.IsTrue(Math.Abs((entries[0].CreatedDate - dateUTtcTimeNow).TotalMinutes) < 1);
                    Assert.IsTrue(Math.Abs((entries[1].CreatedDate - dateUTtcTimeNow).TotalMinutes) < 1);
                    Assert.IsTrue(Math.Abs((entries[2].CreatedDate - dateUTtcTimeNow).TotalMinutes) < 1);
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
                        Assert.AreEqual(6, entries.Count);

                        // Entries State
                        Assert.IsTrue(Math.Abs((entries[0].CreatedDate - dateTimeNow).TotalMinutes) < 1);
                        Assert.IsTrue(Math.Abs((entries[1].CreatedDate - dateTimeNow).TotalMinutes) < 1);
                        Assert.IsTrue(Math.Abs((entries[2].CreatedDate - dateTimeNow).TotalMinutes) < 1);

                        // Entries State
                        Assert.IsTrue(Math.Abs((entries[3].CreatedDate - dateUTtcTimeNow).TotalMinutes) < 1);
                        Assert.IsTrue(Math.Abs((entries[4].CreatedDate - dateUTtcTimeNow).TotalMinutes) < 1);
                        Assert.IsTrue(Math.Abs((entries[5].CreatedDate - dateUTtcTimeNow).TotalMinutes) < 1);
                    }
                }
            }
        }
    }
}
