using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus.Mik_Area
{
	[TestClass]
	public class Hook_Test
	{
		public static void cleannup()
		{
			using (var context = new ModelAndContext.EntityContext())
			{
				context.TvContracts.RemoveRange(context.TvContracts);
				context.MobileContracts.RemoveRange(context.MobileContracts);
				context.BroadbandContracts.RemoveRange(context.BroadbandContracts);
				context.SaveChanges();
			}
		}

		[TestMethod()]
		public void Hook_Test_01()
		{
            cleannup();
            // SEED
            using (var context = new ModelAndContext.EntityContext())
            {
                //4
                context.TvContracts.Add(new ModelAndContext.TvContract() { Months = 4 });
                context.TvContracts.Add(new ModelAndContext.TvContract() { Months = 4 });
                context.TvContracts.Add(new ModelAndContext.TvContract() { Months = 4 });
                context.TvContracts.Add(new ModelAndContext.TvContract() { Months = 4 });

                //4
                context.MobileContracts.Add(new ModelAndContext.MobileContract() { Months = 3, MobileNumber = "4524" });
                context.MobileContracts.Add(new ModelAndContext.MobileContract() { Months = 3, MobileNumber = "4524" });
                context.MobileContracts.Add(new ModelAndContext.MobileContract() { Months = 3, MobileNumber = "4524" });
                context.MobileContracts.Add(new ModelAndContext.MobileContract() { Months = 3, MobileNumber = "4524" });

                //8
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { Months = 6, DownloadSpeed = 50 });
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { DownloadSpeed = 52 });
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { Months = 6, DownloadSpeed = 50 });
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { DownloadSpeed = 52 });
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { Months = 6, DownloadSpeed = 50 });
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { DownloadSpeed = 52 });
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { Months = 6, DownloadSpeed = 50 });
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { DownloadSpeed = 51 });


                context.SaveChanges();
            } 

            using (var context = new ModelAndContext.EntityContext())
            {
                var sql = "";
                context.Database.Log = s => sql += s;
                 var list = context.Contracts.WithHint(SqlServerTableHintFlags.NOLOCK, typeof(ModelAndContext.TvContract),
                    typeof(ModelAndContext.MobileContract), typeof(ModelAndContext.BroadbandContract)).ToList();
                Assert.IsTrue(sql.Contains("NOLOCK"));
                Assert.AreEqual(16, list.Count()); 
            } 
        }

        [TestMethod]
        public void Hook_Test_02()
		{
            using (var dbContext = new ModelAndContext.EntityContext())
			{
				var sql = "";
				dbContext.Database.Log = s => sql += s;
				var changedUpdatedEntityIds = new[] { Guid.NewGuid() };

				var changedJoinedEntity1Ids = new[] { Guid.NewGuid() };
				var changedIndirectlyJoinedEntityIds = new[] { Guid.NewGuid() };

				var changedJoinedEntity2Ids = new[] { Guid.NewGuid() }; 

					var t = dbContext.Set<ModelAndContext.UpdatedEntity>().WithHint(SqlServerTableHintFlags.NOLOCK, 
						typeof(ModelAndContext.UpdatedEntity), typeof(ModelAndContext.UpdatedEntity), typeof(ModelAndContext.IndirectlyJoinedEntity),
						typeof(ModelAndContext.JoinedEntity1), typeof(ModelAndContext.JoinedEntity2)).Include(x => x.JoinedEntity1).
							Include(x => x.JoinedEntity2)
							.Where(
								sp => changedUpdatedEntityIds.Contains(sp.UpdatedEntityId)
									  || changedJoinedEntity2Ids.Contains(sp.JoinedEntity2.JoinedEntity2Id)
									  || changedJoinedEntity1Ids.Contains(sp.JoinedEntity1.JoinedEntity1Id)
									  || changedIndirectlyJoinedEntityIds.Contains(sp.JoinedEntity1.IndirectlyJoinedEntity.IndirectlyJoinedEntityId))
							.Select(
								u => new
								{
									Status = (!u.IsActive
										? ModelAndContext.UpdatedEntityStatus.Inactive
										: u.RestrictedBool && u.JoinedEntity2.String == null
											? ModelAndContext.UpdatedEntityStatus.Invalid
											: u.JoinedEntity1.Status != ModelAndContext.JoinedEntity1Status.Valid
												? ModelAndContext.UpdatedEntityStatus.Invalid
												: u.JoinedEntity1.Restrictions.Restriction1 && u.RestrictedNumber == 0
													? ModelAndContext.UpdatedEntityStatus.Invalid
													: u.JoinedEntity1.Restrictions.Restriction2 && !u.RestrictedBool
														? ModelAndContext.UpdatedEntityStatus.Invalid
														: ModelAndContext.UpdatedEntityStatus.Valid)
								}).ToList();

				// keep try catch, 6 if only this test run, and if two run is 10
				try
				{ 
					Assert.AreEqual(6, sql.Split(new String[] { "NOLOCK" }, StringSplitOptions.None).LongCount());
				}
				catch (Exception e )
				{
					Assert.AreEqual(10, sql.Split(new String[] { "NOLOCK" }, StringSplitOptions.None).LongCount());
				}
				Assert.AreEqual(0, t.Count());

			}
		}
	} 
}
