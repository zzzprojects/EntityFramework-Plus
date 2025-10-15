#if !INMEMORY && EFCORE_6X && !EFCORE_8X
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus.EFCore.Shared.MikaelAreaIndependant
{
	[TestClass]
	public class IncludFilterTimeofDay
	{

		//TESTCoreIncludeFilterSimple_TODO_NewVersionPlusTestForCore
		public static string DatabaseName = "ForUseDateOnlyTimeOnly";

		// [REPLACE] is in Beta.
		public static string ConnectionString =
			("Server=[REPLACE];Initial Catalog = [BD]; Integrated Security = true; Connection Timeout = 300; Persist Security Info=True;TrustServerCertificate=true").Replace("[REPLACE]", Environment.MachineName).Replace("[BD]", DatabaseName);

		public static void Clean()
		{
			using (var context = new EntityContext())
			{

				My.DeleteBD(context); 
				context.Database.EnsureCreated();	
			}
		} 

		[TestMethod()]
		public void IncludFilterTimeofDay_01()
		{
			Clean();
			// SEED  
			using (var context = new EntityContext())
			{
				for (int i = 0; i < 3; i++)
				{
					context.Add(new EntitySimpleTimeOfDay { ColumnInt = i, Childs = new List<EntitySimpleChildTimeOfDay>() { new EntitySimpleChildTimeOfDay() { /*Test = DateTime.Now.AddDays(1),*/ StartDate = new DateOnly(2022, 12, 24), TimeOfDay = new TimeOnly(12, 00), Meta = new EntitySimpleMetaTimeOfDay() { /*Test = DateTime.Now.AddDays(1),*/ StartDate = new DateOnly(2022, 12, 24), TimeOfDay = new TimeOnly(12, 00) } } } });
				}
				for (int i = 0; i < 3; i++)
				{
					context.Add(new EntitySimpleTimeOfDay { ColumnInt = i, Childs = new List<EntitySimpleChildTimeOfDay>() { new EntitySimpleChildTimeOfDay() { /*Test = DateTime.Now.AddDays(1),*/ StartDate = new DateOnly(2020, 12, 24), TimeOfDay = new TimeOnly(12, 00), Meta = new EntitySimpleMetaTimeOfDay() { /*Test = DateTime.Now.AddDays(1),*/ StartDate = new DateOnly(2022, 12, 24), TimeOfDay = new TimeOnly(12, 00) } } } });
				}
				for (int i = 0; i < 3; i++)
				{
					context.Add(new EntitySimpleTimeOfDay { ColumnInt = i, Childs = new List<EntitySimpleChildTimeOfDay>() { new EntitySimpleChildTimeOfDay() { /*Test = DateTime.Now.AddDays(1),*/ StartDate = new DateOnly(2022, 12, 24), TimeOfDay = new TimeOnly(12, 00), Meta = new EntitySimpleMetaTimeOfDay() { /*Test = DateTime.Now.AddDays(1),*/ StartDate = new DateOnly(2020, 12, 24), TimeOfDay = new TimeOnly(12, 00) } } } });
				}

				context.SaveChanges();
			}
			using (var context = new EntityContext())
			{


				var list = context.EntitySimpleTimeOfDays.IncludeFilter(x => x.Childs.Where(user => user.Meta.StartDate > new DateOnly(2021, 12, 24) && user.StartDate > new DateOnly(2021, 12, 24))).ToList();


				Assert.AreEqual(9, list.Count);
				Assert.AreEqual(3, list.SelectMany(x => x.Childs).Count());
				Assert.AreEqual(3, list.SelectMany(x => x.Childs).Select(y => y.Meta).Count());

			} 
		}

		public class EntitySimpleTimeOfDay
		{
			public int ID { get; set; }
			public int ColumnInt { get; set; }
			public string ColumnString { get; set; }
			public List<EntitySimpleChildTimeOfDay> Childs { get; set; }

		}

		public class EntitySimpleChildTimeOfDay
		{
			public int ID { get; set; }
			public int ColumnInt { get; set; }
			public string ColumnString { get; set; }
			public DateOnly StartDate { get; set; }
			public TimeOnly TimeOfDay { get; set; }
			public EntitySimpleMetaTimeOfDay Meta { get; set; }
		}

		public class EntitySimpleMetaTimeOfDay
		{
			public int ID { get; set; }
			public int ColumnInt { get; set; }
			public string ColumnString { get; set; }
			public string ColumnString2 { get; set; }
			public DateOnly StartDate { get; set; }
			public TimeOnly TimeOfDay { get; set; }
		}

		public class EntityContext : DbContext
		{
			public EntityContext()
			{
			}

#if EFCORE_6X
			public DbSet<EntitySimpleTimeOfDay> EntitySimpleTimeOfDays { get; set; }
			public DbSet<EntitySimpleChildTimeOfDay> EntitySimpleChildTimeOfDays { get; set; }
			public DbSet<EntitySimpleMetaTimeOfDay> EntitySimpleMetaTimeOfDays { get; set; }
#endif
			protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			{
				optionsBuilder.UseSqlServer(new SqlConnection(ConnectionString), x => x.UseDateOnlyTimeOnly());

				base.OnConfiguring(optionsBuilder);
			}
		}


	}
}
#endif