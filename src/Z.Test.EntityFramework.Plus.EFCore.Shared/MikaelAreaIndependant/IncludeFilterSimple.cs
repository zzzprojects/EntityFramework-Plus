#if !INMEMORY && !EFCORE_2X
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus.EFCore.Shared.MikaelAreaIndependant
{
	// just watch if that run for now. (no check value)
	[TestClass]
    public class IncludeFilterSimple
	{
		[TestMethod]
		public  void Execute()
		{
			// Create BD 
			using (var context = new EntityContext())
			{
				My.DeleteBD(context);
				context.Database.EnsureCreated();
			}


			// CLEAN  
			using (var context = new EntityContext())
			{
				context.EntitySimpleChilds.RemoveRange(context.EntitySimpleChilds); 
				context.EntitySimpleChildChilds.RemoveRange(context.EntitySimpleChildChilds);
				context.EntitySimpleChildChildChild.RemoveRange(context.EntitySimpleChildChildChild);
				context.EntitySimpleChildChildChildChild.RemoveRange(context.EntitySimpleChildChildChildChild);
				context.EntitySimples.RemoveRange(context.EntitySimples);
				context.EntitySimpleChildList2s.RemoveRange(context.EntitySimpleChildList2s);
				context.EntitySimpleChildList3s.RemoveRange(context.EntitySimpleChildList3s);

				context.SaveChanges();
			}

			// SEED  
			using (var context = new EntityContext())
			{
				for (int i = 0; i < 3; i++)
				{
					context.EntitySimples.Add(new EntitySimple { EntitySimpleChild2 = new EntitySimpleChild2<EntitySimpleChildList2>() { new EntitySimpleChildList2() { ColumnInt = i }, new EntitySimpleChildList2() { ColumnInt = i } },
						
						ColumnInt = i , EntitySimpleChildList = new EntitySimpleChildList() { new EntitySimpleChildList3() {ColumnInt = i}, new EntitySimpleChildList3() { ColumnInt = i } } });
				}

				context.SaveChanges();
			}


			// TEST  
			using (var context = new EntityContext())
			{
				EntitySimple test = null;
				bool hasError = false;
				try
				{
					test = context.EntitySimples.Where(x => false).IncludeFilter(x => x.EntitySimpleChild).First();

				}
				catch
				{
					hasError = true;
				}

				Assert.IsNull(test);
				Assert.IsTrue(hasError);	

				hasError = false;
				try
				{
					test = context.EntitySimples.Where(x => false).IncludeFilter(x => x.EntitySimpleChild).Single();

				}
				catch
				{
					hasError = true;
				}
				Assert.IsNull(test);
				Assert.IsTrue(hasError);

				hasError = false;
				try
				{
					test = context.EntitySimples.Where(x => false).IncludeFilter(x => x.EntitySimpleChild).FirstOrDefault();

				}
				catch
				{
					hasError = true;
				}
				Assert.IsNull(test);
				Assert.IsFalse(hasError);
				try
				{
					test = context.EntitySimples.Where(x => true).IncludeFilter(x => x.EntitySimpleChild).First();

				}
				catch
				{
					hasError = true;
				}
				Assert.IsNotNull(test);
				Assert.IsFalse(hasError);

			}

			// TEST  
			using (var context = new EntityContext())
			{
				var t = context.EntitySimples.IncludeFilter(x => x.EntitySimpleChild).ToList();
				var dasdat = context.EntitySimples.IncludeFilter(x => x.EntitySimpleChild2).ToList();
				Z.EntityFramework.Plus.QueryIncludeFilterManager.AllowQueryBatch = false;
				var tg = context.EntitySimples.AsNoTracking().IncludeFilter(x => x.EntitySimpleChild).ToList();
				var datg = context.EntitySimples.AsNoTracking().IncludeFilter(x => x.EntitySimpleChild2).ToList();
			}


			// SEED  
			using (var context = new EntityContext())
			{
				for (int i = 0; i < 3; i++)
				{
					context.EntitySimples.Add(new EntitySimple
					{
						ColumnInt = i,
						EntitySimpleChilds = new List<EntitySimpleChild>() { new EntitySimpleChild() { ColumnInt = i }, new EntitySimpleChild() { ColumnInt = i } },

						EntitySimpleChildChilds = new List<EntitySimpleChildChild>() { new EntitySimpleChildChild() { ColumnInt = i,EntitySimpleChildChildChilds = new List<EntitySimpleChildChildChild>() {new EntitySimpleChildChildChild() { ColumnInt = i}, new EntitySimpleChildChildChild() { ColumnInt = i } }
						}, new EntitySimpleChildChild() { ColumnInt = i ,EntitySimpleChildChildChilds = new List<EntitySimpleChildChildChild>() {new EntitySimpleChildChildChild() { ColumnInt = i}, new EntitySimpleChildChildChild() { ColumnInt = i } }} }
					});
				}

				context.SaveChanges();
			}
			// TEST  
			using (var context = new EntityContext())
			{
				var t = context.EntitySimples.AsNoTracking().IncludeFilter(x => x.EntitySimpleChilds).ToList();
				var s = "a";
				var aata = context.EntitySimples.AsNoTracking().IncludeFilter(x => x.EntitySimpleChilds.Where(x => !x.ColumnString.Contains(s) && !x.ColumnInt.ToString().Contains(s))).ToList();
				var ta = context.EntitySimples.AsNoTracking().IncludeFilter(x => x.EntitySimpleChilds).Where(x => !x.ColumnString.Contains(s) && !x.ColumnInt.ToString().Contains(s)).ToList();
				var ttt = context.EntitySimples.AsNoTracking().IncludeFilter(x => x.EntitySimpleChilds).IncludeFilter(x => x.EntitySimpleChildChilds).ToList();
				var ttat = context.EntitySimples.AsNoTracking().IncludeFilter(x => x.EntitySimpleChilds).IncludeFilter(x => x.EntitySimpleChildChilds).IncludeFilter(x => x.EntitySimpleChildChilds.SelectMany(x => x.EntitySimpleChildChildChilds)).ToList();
				var ttfsfsat = context.EntitySimples.AsNoTracking().IncludeFilter(x => x.EntitySimpleChildChilds.SelectMany(x => x.EntitySimpleChildChildChilds).SelectMany(y => y.EntitySimpleChildChildChilds)).ToList();
				
				 
			}

			// TEST  
			using (var context = new EntityContext())
			{
				var s = "a";
				Z.EntityFramework.Plus.QueryIncludeFilterManager.AllowQueryBatch = false;
				var taa = context.EntitySimples.AsNoTracking().IncludeFilter(x => x.EntitySimpleChilds).Where(x => !x.ColumnString.Contains(s) && !x.ColumnInt.ToString().Contains(s)).ToList();
				var tasda = context.EntitySimples.AsNoTracking().IncludeFilter(x => x.EntitySimpleChilds.Where(x => !x.ColumnString.Contains(s) && !x.ColumnInt.ToString().Contains(s))).ToList();
				var tt = context.EntitySimples.AsNoTracking().IncludeFilter(x => x.EntitySimpleChilds).ToList();
				var tttt = context.EntitySimples.AsNoTracking().IncludeFilter(x => x.EntitySimpleChilds).IncludeFilter(x => x.EntitySimpleChildChilds).ToList();
				var tatt = context.EntitySimples.AsNoTracking().IncludeFilter(x => x.EntitySimpleChilds).IncludeFilter(x => x.EntitySimpleChildChilds).IncludeFilter(x => x.EntitySimpleChildChilds.SelectMany(x => x.EntitySimpleChildChildChilds)).ToList();
			}
		}

		public class EntityContext : DbContext
		{
			public EntityContext()
			{
			} 
			public DbSet<EntitySimpleChild> EntitySimpleChilds { get; set; } 
			public DbSet<EntitySimpleChildChild> EntitySimpleChildChilds { get; set; }
			public DbSet<EntitySimpleChildChildChild> EntitySimpleChildChildChild { get; set; }
			public DbSet<EntitySimpleChildChildChildChild> EntitySimpleChildChildChildChild { get; set; }
			public DbSet<EntitySimple> EntitySimples { get; set; }
			public DbSet<EntitySimpleChildList2> EntitySimpleChildList2s { get; set; }
			public DbSet<EntitySimpleChildList3> EntitySimpleChildList3s { get; set; }

			protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			{
				optionsBuilder.UseSqlServer(new SqlConnection(My.ConnectionString));

				base.OnConfiguring(optionsBuilder);
			}
		}

		public class EntitySimple
		{
			public int ID { get; set; }
			public int ColumnInt { get; set; }
			public string ColumnString { get; set; }
			public EntitySimpleChild EntitySimpleChild { get;set; }
			public EntitySimpleChildList EntitySimpleChildList { get; set; }
			public EntitySimpleChild2<EntitySimpleChildList2> EntitySimpleChild2 { get; set; }
			public List<EntitySimpleChild> EntitySimpleChilds { get; set; }
			public List<EntitySimpleChildChild> EntitySimpleChildChilds { get; set; }
		}

		public class EntitySimpleChildList : List<EntitySimpleChildList3>
		{
		}

		public class EntitySimpleChild2<T> : List<T>
		{
		}

		public class EntitySimpleChildList3
		{
			public int ID { get; set; }
			public int ColumnInt { get; set; }
			public string ColumnString { get; set; }
		}

		public class EntitySimpleChildList2
		{
			public int ID { get; set; }
			public int ColumnInt { get; set; }
			public string ColumnString { get; set; }
		}

		public class EntitySimpleChild
		{
			public int ID { get; set; }
			public int ColumnInt { get; set; }
			public string ColumnString { get; set; }
		}

		public class EntitySimpleChildChild
		{
			public int ID { get; set; }
			public int ColumnInt { get; set; }
			public string ColumnString { get; set; }
			public List<EntitySimpleChildChildChild> EntitySimpleChildChildChilds { get; set; }
		}

		public class EntitySimpleChildChildChild
		{
			public int ID { get; set; }
			public int ColumnInt { get; set; }
			public string ColumnString { get; set; }
			public List<EntitySimpleChildChildChildChild> EntitySimpleChildChildChilds { get; set; }
		}

		public class EntitySimpleChildChildChildChild
		{
			public int ID { get; set; }
			public int ColumnInt { get; set; }
			public string ColumnString { get; set; }
		}
	}
}
#endif