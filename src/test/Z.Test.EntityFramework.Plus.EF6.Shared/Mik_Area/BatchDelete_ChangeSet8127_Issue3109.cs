using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus.Mik_Area
{
	[TestClass]
	public class BatchDelete_ChangeSet8127_Issue3109
	{
		public static void Cleannup()
		{
			using (var context = new ModelAndContext.EntityContext())
			{
				context.Companies.Delete();
				context.Orders.Delete();
				context.Customers.Delete();
			}
		}

		public static void Seed()
		{
			using (var context = new ModelAndContext.EntityContext())
			{
			 	var Company = new ModelAndContext.Company() {Name = "DoesntWork" };
				var Company2 = new ModelAndContext.Company() { Name = "DoesntWorkZ" };
				var order1 = new ModelAndContext.Order() {Name = "Works"};
				var order2 = new ModelAndContext.Order() { Name = "NotWorks" };
				 order2.Company = Company;
				 order1.Company = Company2;
				context.Orders.Add(order1);
				context.Orders.Add(order2);
			 
				context.Customers.Add(new ModelAndContext.Customer() { Name = "Customer_A", IsActive = false, Orders = new List<ModelAndContext.Order>() {order1}});
				context.Customers.Add(new ModelAndContext.Customer() { Name = "Customer_B", IsActive = true, Orders = new List<ModelAndContext.Order>() { order2 } });
				context.Customers.Add(new ModelAndContext.Customer() { Name = "Customer_C", IsActive = false });
				context.SaveChanges();

			}
		}

		[TestMethod]
		public void BatchUpdate_1()
		{
			Cleannup();
			Seed();
			using (var context = new ModelAndContext.EntityContext())
			{

				context.Customers.Update(c => new ModelAndContext.Customer { TotalNumberOfOrders = c.Orders.Count(o => o.Name == "Works") });

				Assert.AreEqual(1,context.Customers.Where(x => x.TotalNumberOfOrders == 1 && x.Name == "Customer_A").ToList().Count);
				Assert.AreEqual(0, context.Customers.Where(x => x.TotalNumberOfOrders == 1 && x.Name !="Customer_A").ToList().Count);
				Assert.AreEqual(2, context.Customers.Where(x => x.TotalNumberOfOrders != 1 && x.Name != "Customer_A").ToList().Count);

			} 

			using (var context = new ModelAndContext.EntityContext())
			{ 
				context.Customers.Update(c => new ModelAndContext.Customer { TotalNumberOfOrders = c.Orders.Count(o => o.Company.Name == "DoesntWork") });


				Assert.AreEqual(1, context.Customers.Where(x => x.TotalNumberOfOrders == 1 && x.Name == "Customer_B").ToList().Count);
				Assert.AreEqual(0, context.Customers.Where(x => x.TotalNumberOfOrders == 1 && x.Name != "Customer_B").ToList().Count);
				Assert.AreEqual(2, context.Customers.Where(x => x.TotalNumberOfOrders != 1 && x.Name != "Customer_B").ToList().Count);
			}

			 
		}

		[TestMethod]
		public void BatchUpdate_2()
		{
			Cleannup();
			Seed();
			var changedUpdatedEntityIds = new[] { Guid.NewGuid() };

			var changedJoinedEntity1Ids = new[] { Guid.NewGuid() };
			var changedIndirectlyJoinedEntityIds = new[] { Guid.NewGuid() };

			var changedJoinedEntity2Ids = new[] { Guid.NewGuid() };
			using (var context = new ModelAndContext.EntityContext())
			{
				 
				context.Customers.Update(c => new ModelAndContext.Customer { TotalNumberOfOrders = c.Orders.Count(o => o.Name == "Works") });
				 

				context.Customers.Update(c => new ModelAndContext.Customer { TotalNumberOfOrders = c.Orders.Count(o => o.Company.Name == "DoesntWork") });


				// TODO CHECK VALUE !
				context.Set<ModelAndContext.UpdatedEntity>().Include(x => x.JoinedEntity1).Include(x => x.JoinedEntity2)
					.Where(
						sp => changedUpdatedEntityIds.Contains(sp.UpdatedEntityId)
						      || changedJoinedEntity2Ids.Contains(sp.JoinedEntity2.JoinedEntity2Id)
						      || changedJoinedEntity1Ids.Contains(sp.JoinedEntity1.JoinedEntity1Id)
						      || changedIndirectlyJoinedEntityIds.Contains(sp.JoinedEntity1.IndirectlyJoinedEntity.IndirectlyJoinedEntityId))
					.Update(
						u => new ModelAndContext.UpdatedEntity
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
						});
			}
		}
	}

}
