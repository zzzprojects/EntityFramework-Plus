#if !EFCORE_2X
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus.EFCore.Shared.MikaelAreaIndependant
{
	[TestClass]
	public class IncludeOptimized
	{
		[TestMethod()]
		public void IncludeOptimized_1()
		{
			Action action = () =>
			{
				// CLEAN  
				using (var context = new ModelAndContext.EntityContext())
				{
					context.Entities.RemoveRange(context.Entities);

					context.SaveChanges();
				}

				// SEED  
				using (var context = new ModelAndContext.EntityContext())
				{
					context.Entities.Add(new Derived { Includes = new List<DerivedInclude>() { new DerivedInclude() { SomeProp = "test" }, { new DerivedInclude() { SomeProp = "test" } } } });
					context.SaveChanges();
				}
				List<Entity> entityEF = new List<Entity>();
				List<Entity> entityEFPlus = new List<Entity>();

				// TEST  
				using (var context = new ModelAndContext.EntityContext())
				{
					entityEF = context.Entities.Include("Includes").ToList();
				} 

				// TEST  
				using (var context = new ModelAndContext.EntityContext())
				{
					entityEFPlus = context.Entities.IncludeOptimizedByPath("Includes").ToList();
				}

				 Assert.AreEqual(((Derived)entityEF.First()).Includes.Count, ((Derived)entityEFPlus.First()).Includes.Count);
			};

			MyIni.RunWithFailLogical(MyIni.GetSetupCasTest(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name), action);
		}


		public static void GenerateData()
		{

			using (var context = new ModelAndContext.EntityContext())
			{
				for (int i = 0; i < 3; i++)
				{
					context.Add(new EntitySimpleForPath() {ColumnInt = i, ColumnString = "EntitySimpleForPath",
						EntitySimpleForPathChild1 = new EntitySimpleForPathChild1()
						{
							ColumnInt = i,
							ColumnString = "EntitySimpleForPathChild1"
						},
						EntitySimpleForPathChild2 = new EntitySimpleForPathChild2()
						{
							ColumnInt = i,
							ColumnString = "EntitySimpleForPathChild2"
						},
						EntitySimpleForPathChild3 = new EntitySimpleForPathChild3()
						{
							ColumnInt = i,
							ColumnString = "EntitySimpleForPathChild3"
						},
						EntitySimpleForPathChildList1s = new List<EntitySimpleForPathChildList1>()
						{
							new EntitySimpleForPathChildList1() {ColumnInt = i, ColumnString = "EntitySimpleForPathChildList1" },

							new EntitySimpleForPathChildList1() {ColumnInt = i, ColumnString = "EntitySimpleForPathChildList1"},

							new EntitySimpleForPathChildList1() {ColumnInt = i, ColumnString = "EntitySimpleForPathChildList1" }
						},
						EntitySimpleForPathChildList2s = new List<EntitySimpleForPathChildList2>()
						{
							new EntitySimpleForPathChildList2() {ColumnInt = i, ColumnString = "EntitySimpleForPathChildList2"},

							new EntitySimpleForPathChildList2() {ColumnInt = i, ColumnString = "EntitySimpleForPathChildList2" },

							new EntitySimpleForPathChildList2() {ColumnInt = i, ColumnString = "EntitySimpleForPathChildList2" }
						},
						EntitySimpleForPathChildList3s = new List<EntitySimpleForPathChildList3>()
						{
							new EntitySimpleForPathChildList3() {ColumnInt = i, ColumnString = "EntitySimpleForPathChildList3" },

							new EntitySimpleForPathChildList3() {ColumnInt = i, ColumnString = "EntitySimpleForPathChildList3" },

							new EntitySimpleForPathChildList3() {ColumnInt = i, ColumnString = "EntitySimpleForPathChildList3" }
						}
					});
				}

				context.SaveChanges();
			}
		}

		public static void CleanData()
		{
			using (var context = new ModelAndContext.EntityContext())
			{
				context.RemoveRange(context.EntitySimpleForPaths);
				context.RemoveRange(context.EntitySimpleForPathChild1s);
				context.RemoveRange(context.EntitySimpleForPathChild2s);
				context.RemoveRange(context.EntitySimpleForPathChild3s);
				context.RemoveRange(context.EntitySimpleForPathChildList1s);
				context.RemoveRange(context.EntitySimpleForPathChildList2s);
				context.RemoveRange(context.EntitySimpleForPathChildList3s); 
				context.SaveChanges();
			}
		}

		[TestMethod()]
		public void IncludeOptimized_ByPath_1()
		{
			CleanData();
			GenerateData();  
			 
			using (var context = new ModelAndContext.EntityContext())
			{
				// LOAD blogs and related active posts and comments.
				var list = context.EntitySimpleForPaths.IncludeOptimizedByPath("EntitySimpleForPathChild1").IncludeOptimizedByPath("EntitySimpleForPathChild2")
						.IncludeOptimizedByPath("EntitySimpleForPathChild3")
					.ToList();

				Assert.IsNotNull(list);
				Assert.AreEqual("EntitySimpleForPathChild3", list.First().EntitySimpleForPathChild3.ColumnString);
			}
		}

		[TestMethod()]
		public void IncludeOptimized_ByPath_2()
		{
			CleanData();
			GenerateData();

			using (var context = new ModelAndContext.EntityContext())
			{
				// LOAD blogs and related active posts and comments.
				var list = context.EntitySimpleForPaths.IncludeOptimizedByPath("EntitySimpleForPathChildList2s").IncludeOptimizedByPath("EntitySimpleForPathChildList3s")
					.IncludeOptimizedByPath("EntitySimpleForPathChildList2s")
					.ToList();

				Assert.IsNotNull(list);
				Assert.AreEqual("EntitySimpleForPathChildList2", list.First().EntitySimpleForPathChildList2s.First().ColumnString);
			}
		}
	}
}


#endif