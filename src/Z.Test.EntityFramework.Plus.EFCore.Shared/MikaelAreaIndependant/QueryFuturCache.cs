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
	public class QueryFuturCache
	{
		[TestMethod()]
		public void QueryFuturCache_1()
		{
			Action action = () =>
			{
				// CLEAN  
				using (var context = new ModelAndContext.EntityContext())
				{
					context.EntitySimpleCacheOnlyDontNewUses.RemoveRange(context.EntitySimpleCacheOnlyDontNewUses);

					context.SaveChanges();
				}


				// SEED  
				using (var context = new ModelAndContext.EntityContext())
				{
					for (int i = 0; i < 4; i++)
					{
						context.EntitySimpleCacheOnlyDontNewUses.Add(new ModelAndContext.EntitySimpleCacheOnlyDontNewUse() { ColumnInt = i, ColumnString = i.ToString() });
					}

					context.SaveChanges();
				}


				// TEST  
				using (var context = new ModelAndContext.EntityContext())
				{
					QueryFutureValue<string> value1;
					QueryFutureValue<string> value2;
					string value3;
					string value4;

					{
						var columnInt = 0;
						value1 = context.EntitySimpleCacheOnlyDontNewUses.Where(x => x.ColumnInt == columnInt).Select(x => x.ColumnString).FutureValue();
					}

					{
						var columnInt = 1;
						value2 = context.EntitySimpleCacheOnlyDontNewUses.Where(x => x.ColumnInt == columnInt).Select(x => x.ColumnString).FutureValue();
					}

					{
						var columnInt = 2;
						// execute an implicit string, so value4 become a solo FutureValue
						value3 = context.EntitySimpleCacheOnlyDontNewUses.Where(x => x.ColumnInt == columnInt).Select(x => x.ColumnString).FutureValue();
					}

					{
						var columnInt = 3;
						value4 = context.EntitySimpleCacheOnlyDontNewUses.Where(x => x.ColumnInt == columnInt).Select(x => x.ColumnString).FutureValue();
					}


					Assert.AreEqual("0", value1.Value);
					Assert.AreEqual("1", value2.Value);
					Assert.AreEqual("2", value3);
					Assert.AreEqual("3", value4); 

				}

				// TEST  
				using (var context = new ModelAndContext.EntityContext())
				{
					var columnInt = 3;
					var value = context.EntitySimpleCacheOnlyDontNewUses.Where(x => x.ColumnInt == columnInt).Select(x => x.ColumnString).FutureValue().Value;

					Assert.AreEqual("3", value);
				}

	#if !EFCORE_2X

				// TEST  
				using (var context = new ModelAndContext.EntityContext())
				{
					var columnInt = 6;
					var value = context.EntitySimpleCacheOnlyDontNewUses.Where(x => x.ColumnInt == columnInt).Select(x => x.ColumnString).FutureValue().Value;

					Assert.IsNull(value);
				}
	#endif

				// TEST  
				using (var context = new ModelAndContext.EntityContext())
				{
					var columnInt = 3;
					var list = context.EntitySimpleCacheOnlyDontNewUses.Where(x => x.ColumnInt == columnInt).Select(x => x.ColumnString).Future().ToList();

					Assert.AreEqual(1, list.Count);
				}


				// TEST  
				using (var context = new ModelAndContext.EntityContext())
				{
					var columnInt = 3;
					var list = context.EntitySimpleCacheOnlyDontNewUses.Where(x => x.ColumnInt < columnInt).Select(x => x.ColumnString).Future().ToList();

					Assert.AreEqual(3, list.Count);
				}
			};

			MyIni.RunWithFailLogical(MyIni.GetSetupCasTest(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name), action);
		}

#if !EFCORE_2X
		[TestMethod()]
		public void QueryFuturCache_2()
		{ 
			// CLEAN  
			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimpleCacheOnlyDontNewUses.RemoveRange(context.EntitySimpleCacheOnlyDontNewUses);

				context.SaveChanges();
			}


			// SEED  
			using (var context = new ModelAndContext.EntityContext())
			{
				for (int i = 0; i < 4; i++)
				{
					context.EntitySimpleCacheOnlyDontNewUses.Add(new ModelAndContext.EntitySimpleCacheOnlyDontNewUse() { ColumnInt = i, ColumnString = i.ToString() });
				}

				context.SaveChanges();
			}


			// TEST  
			using (var context = new ModelAndContext.EntityContext())
			{
				QueryFutureValue<string> value1;
				QueryFutureValue<string> value2; 

				{
					var columnInt = 0;
					value1 = context.EntitySimpleCacheOnlyDontNewUses.Where(x => x.ColumnInt == columnInt).Select(x => x.ColumnString).FutureValue();
				}

				{
					var columnInt = 1;
					value2 = context.EntitySimpleCacheOnlyDontNewUses.Where(x => x.ColumnInt == columnInt).Select(x => x.ColumnString).FutureValue();
				} 

				QueryFutureManager.ExecuteBatch(context); 

				Assert.AreEqual("0", value1);
				Assert.AreEqual("1", value2); 

			} 
		}

		[TestMethod()]
		public void QueryFuturCache_3()
		{
			// CLEAN  
			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimpleCacheOnlyDontNewUses.RemoveRange(context.EntitySimpleCacheOnlyDontNewUses);

				context.SaveChanges();
			}


			// SEED  
			using (var context = new ModelAndContext.EntityContext())
			{
				for (int i = 0; i < 4; i++)
				{
					context.EntitySimpleCacheOnlyDontNewUses.Add(new ModelAndContext.EntitySimpleCacheOnlyDontNewUse() { ColumnInt = i, ColumnString = i.ToString() });
				}

				context.SaveChanges();
			}


			// TEST  
			using (var context = new ModelAndContext.EntityContext())
			{
				QueryFutureValue<string> value1;
				QueryFutureValue<string> value2; 

				{
					var columnInt = 0;
					value1 = context.EntitySimpleCacheOnlyDontNewUses.Where(x => x.ColumnInt == columnInt).Select(x => x.ColumnString).FutureValue();
				}

				{
					var columnInt = 1;
					value2 = context.EntitySimpleCacheOnlyDontNewUses.Where(x => x.ColumnInt == columnInt).Select(x => x.ColumnString).FutureValue();
				} 

				QueryFutureManager.ExecuteBatchAsync(context).Wait();

				Assert.AreEqual("0", value1);
				Assert.AreEqual("1", value2); 

			} 
		}
#endif
	}
}
