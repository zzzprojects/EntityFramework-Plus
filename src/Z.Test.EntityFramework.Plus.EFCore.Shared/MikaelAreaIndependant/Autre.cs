#if !INMEMORY
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
	public class Autre
	{
		public static void Clean()
		{
			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimples.RemoveRange(context.EntitySimples);
				context.SaveChanges();
			}
		}
		public class IsFinish
		{
			public bool IsFinished
            {
				get;set;
            }
		}
		[TestMethod()]
		public void ListString_01()
		{
			Clean();
			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimples.Add(new EntitySimple() { ColumnInt = 10 });
				context.EntitySimples.Add(new EntitySimple() { ColumnInt = 2, ColumnString = "testaa" });
				context.EntitySimples.Add(new EntitySimple() { ColumnInt = 2, ColumnString = "allo" });
				context.EntitySimples.Add(new EntitySimple() { ColumnInt = 20, ColumnString = "testaa" });
				context.SaveChanges();
			}
			using (var context = new ModelAndContext.EntityContext())
			{

				List<string> list = new List<string>() { "testaa", "patate" };

				var t = context.EntitySimples.Where(x => list.Contains(x.ColumnString)).DeferredCount().FutureValue();
				var ta = context.EntitySimples.Where(x => !list.Contains(x.ColumnString) && x.ColumnInt == 2).DeferredCount().FutureValue();
				var tr = context.EntitySimples.Where(x => list.Contains(x.ColumnString) && x.ColumnInt == 2).DeferredCount().FutureValue().Value;

				Assert.AreEqual(2, t.Value);
				Assert.AreEqual(1, ta.Value);
				Assert.AreEqual(1, tr);
            } 
            using (var context = new ModelAndContext.EntityContext())
			{

				List<string> list = new List<string>() { "testaa", "patate" };

				var t = context.EntitySimples.Where(x => list.Contains(x.ColumnString)).Future();
				var ta = context.EntitySimples.Where(x => !list.Contains(x.ColumnString) && x.ColumnInt == 2).Future();
				var tr = context.EntitySimples.Where(x => list.Contains(x.ColumnString) && x.ColumnInt == 2).Future().ToList();

				Assert.AreEqual(2, t.Count());
				Assert.AreEqual(1, ta.Count());
				Assert.AreEqual(1, tr.Count());
				Assert.AreEqual("allo", ta.First().ColumnString);
				Assert.AreEqual("testaa", tr.First().ColumnString);
            }

        }
        [TestMethod()]
        public void ListString_02()
        {
            Clean();
            using (var context = new ModelAndContext.EntityContext())
            {
                context.EntitySimples.Add(new EntitySimple() { ColumnInt = 10 });
                context.EntitySimples.Add(new EntitySimple() { ColumnInt = 2, ColumnString = "testaa" });
                context.EntitySimples.Add(new EntitySimple() { ColumnInt = 2, ColumnString = "allo" });
                context.EntitySimples.Add(new EntitySimple() { ColumnInt = 20, ColumnString = "testaa" });
                context.SaveChanges();
            }
            using (var context = new ModelAndContext.EntityContext())
            {

                List<string> list = new List<string>() { "testaa", "patate" };
                List<string> list2 = new List<string>() { "testaa", "allo" };

                var t = context.EntitySimples.Where(x => list.Contains(x.ColumnString)).FromCache();
                var ta = context.EntitySimples.Where(x => !list.Contains(x.ColumnString) && x.ColumnInt == 2).FromCache();
                var tr = context.EntitySimples.Where(x => list.Contains(x.ColumnString) && x.ColumnInt == 2).FromCache();
                list = list2;
                var t2 = context.EntitySimples.Where(x => list.Contains(x.ColumnString)).FromCache();
                var ta2 = context.EntitySimples.Where(x => !list.Contains(x.ColumnString) && x.ColumnInt == 2).FromCache();
                var tr2 = context.EntitySimples.Where(x => list.Contains(x.ColumnString) && x.ColumnInt == 2).FromCache();

                Assert.AreEqual(2, t.Count());
                Assert.AreEqual(1, ta.Count());
                Assert.AreEqual(1, tr.Count());
                Assert.AreEqual("allo", ta.First().ColumnString);
                Assert.AreEqual("testaa", tr.First().ColumnString);
                Assert.AreEqual(3, t2.Count());
                Assert.AreEqual(0, ta2.Count());
                Assert.AreEqual(2, tr2.Count()); 
            } 

        }
        [TestMethod()]
        public void ListString_03()
        {
            Clean();
            using (var context = new ModelAndContext.EntityContext())
            {
                context.EntitySimples.Add(new EntitySimple() { ColumnInt = 10 });
                context.EntitySimples.Add(new EntitySimple() { ColumnInt = 2, ColumnString = "testaa" });
                context.EntitySimples.Add(new EntitySimple() { ColumnInt = 2, ColumnString = "allo" });
                context.EntitySimples.Add(new EntitySimple() { ColumnInt = 20, ColumnString = "testaa" });
                context.SaveChanges();
            }
            using (var context = new ModelAndContext.EntityContext())
            {

                List<string> list = new List<string>() { "testaa", "patate" };
                List<string> list2 = new List<string>() { "testaa", "allo" };

                var t = context.EntitySimples.Where(x => list.Contains(x.ColumnString)).Future();
                var ta = context.EntitySimples.Where(x => !list.Contains(x.ColumnString) && x.ColumnInt == 2).Future();
                var tr = context.EntitySimples.Where(x => list.Contains(x.ColumnString) && x.ColumnInt == 2).Future();
                list = list2;
                var t2 = context.EntitySimples.Where(x => list.Contains(x.ColumnString)).Future();
                var ta2 = context.EntitySimples.Where(x => !list.Contains(x.ColumnString) && x.ColumnInt == 2).Future();
                var tr2 = context.EntitySimples.Where(x => list.Contains(x.ColumnString) && x.ColumnInt == 2).Future().ToList();

                Assert.AreEqual(3, t.Count());
                Assert.AreEqual(0, ta.Count());
                Assert.AreEqual(2, tr.Count());
                Assert.AreEqual(3, t2.Count());
                Assert.AreEqual(0, ta2.Count());
                Assert.AreEqual(2, tr2.Count());
            }

        }

        [TestMethod()]
		public void Asyn_TransactionScope_01()
		{
			Clean();
			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimples.Add(new EntitySimple() { ColumnInt = 10});
				context.EntitySimples.Add(new EntitySimple() { ColumnInt = 20 });
				context.SaveChanges();
			}
			using (var context = new ModelAndContext.EntityContext())
			{
				var isFinish = new IsFinish();
				var task = Test1(isFinish);
				Task.WaitAll(task);
				Assert.IsTrue(isFinish.IsFinished);
			}

		}
		public async static Task Test1(IsFinish isFinish)
		{ 

			using (var context = new ModelAndContext.EntityContext())
			{

				using (var scuope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
				{

					var futureTotalCount = context.EntitySimples.DeferredSum(x => x.ColumnInt).FutureValue();
					var futureItems = context.EntitySimples.AsNoTracking().Future();
					var items = await futureItems.ToListAsync();
					context.EntitySimples.UpdateFromQuery(x => new EntitySimple() { ColumnInt = 10 });
					//scuope.Complete();
					scuope.Dispose();

					Assert.AreEqual(30, futureTotalCount.Value);

					Assert.AreEqual(2, items.Count);
				} 



				using (var scuopasde = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
				{

					var futuresadTotalCount = context.EntitySimples.DeferredSum(x => x.ColumnInt).FutureValue();
					var iteasdms = await context.EntitySimples.AsNoTracking().Future().ToListAsync();

					Assert.AreEqual(30, futuresadTotalCount.Value);

					Assert.AreEqual(2, iteasdms.Count);
				}
			}

			isFinish.IsFinished = true;
		}

		[TestMethod()]
		public void Asyn_TransactionScope_02()
		{
			Clean();
			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimples.Add(new EntitySimple() { ColumnInt = 10 });
				context.EntitySimples.Add(new EntitySimple() { ColumnInt = 20 });
				context.SaveChanges();
			}
			using (var context = new ModelAndContext.EntityContext())
			{
				var isFinish = new IsFinish();
				var task = Test2(isFinish);
				Task.WaitAll(task);
				Assert.IsTrue(isFinish.IsFinished);
			}

		}
		public async static Task Test2(IsFinish isFinish)
		{

			using (var context = new ModelAndContext.EntityContext())
			{

				using (var scuope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
				{

					var futureTotalCount = context.EntitySimples.DeferredSum(x => x.ColumnInt).FutureValue();
					var futureItems = context.EntitySimples.AsNoTracking().Future();
					var items = await futureItems.ToListAsync();
					context.EntitySimples.UpdateFromQuery(x => new EntitySimple() { ColumnInt = 10 });

					scuope.Complete();
					scuope.Dispose();

					Assert.AreEqual(30, futureTotalCount.Value);

					Assert.AreEqual(2, items.Count);

				}


				using (var scuopasde = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
				{

					var futuresadTotalCount = context.EntitySimples.DeferredSum(x => x.ColumnInt).FutureValue();
					var iteasdms = await context.EntitySimples.AsNoTracking().Future().ToListAsync();

					Assert.AreEqual(20, futuresadTotalCount.Value);

					Assert.AreEqual(2, iteasdms.Count);
				}
			}

			isFinish.IsFinished = true;
		}

        [TestMethod()]
        public void Test_SetIdentity()
        {
			try
			{

				Clean();
				using (var context = new ModelAndContext.EntityContext())
				{
					try
					{

						context.EntitySimples.Add(new EntitySimple() { ID = 800000, ColumnInt = 10 });
						context.SaveChanges();
					}
					catch (Exception e)
					{
						context.ChangeTracker.Entries().ToList().ForEach(x => x.State = EntityState.Unchanged);
						context.EntitySimples.SqlServerSetIdentityInsertOn();
						context.EntitySimples.Add(new EntitySimple() { ID = 800001, ColumnInt = 10 });
						context.SaveChanges();
						context.EntitySimples.SqlServerSetIdentityInsertOff();
                    }
				}
			 
				using (var context = new ModelAndContext.EntityContext())
				{
					var t = context.EntitySimples.Single();

					Assert.AreEqual(800001, t.ID);
                }
                Clean();
                using (var context = new ModelAndContext.EntityContext())
                {
                    try
                    {

                        context.EntitySimples.Add(new EntitySimple() { ID = 800000, ColumnInt = 10 });
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        context.ChangeTracker.Entries().ToList().ForEach(x => x.State = EntityState.Unchanged);
                        context.SqlServerSetIdentityInsertOn<EntitySimple>();
                        context.EntitySimples.Add(new EntitySimple() { ID = 800001, ColumnInt = 10 });
                        context.SaveChanges();
                        context.SqlServerSetIdentityInsertOff<EntitySimple>();
                    }
                }

                using (var context = new ModelAndContext.EntityContext())
                {
                    var t = context.EntitySimples.Single();

                    Assert.AreEqual(800001, t.ID);
                }
                Clean();
                using (var context = new ModelAndContext.EntityContext())
                {
                    try
                    {

                        context.EntitySimples.Add(new EntitySimple() { ID = 800000, ColumnInt = 10 });
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        context.ChangeTracker.Entries().ToList().ForEach(x => x.State = EntityState.Unchanged);
                        context.SqlServerSetIdentityInsertOn(typeof(EntitySimple));
                        context.EntitySimples.Add(new EntitySimple() { ID = 800001, ColumnInt = 10 });
                        context.SaveChanges();
                        context.SqlServerSetIdentityInsertOff(typeof(EntitySimple));
                    }
                }

                using (var context = new ModelAndContext.EntityContext())
                {
                    var t = context.EntitySimples.Single();

                    Assert.AreEqual(800001, t.ID);
                }
                using (var context = new ModelAndContext.EntityContext())
                {
					bool hasError = false;
                    try
                    {

                        context.SqlServerSetIdentityInsertOn(typeof(EntitySimple));
                        context.SqlServerSetIdentityInsertOff(typeof(EntitySimple));
                        context.EntitySimples.Add(new EntitySimple() { ID = 800001, ColumnInt = 10 });
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
						hasError = true;
                    }
					Assert.IsTrue(hasError);
                } 

            }
            finally
            {
     //           using (var context = new ModelAndContext.EntityContext())
     //           {
					//context.EntitySimples.SqlServerSetIdentityInsertOff(); 
     //           }
            }

        }

        [TestMethod()]
        public void Test_AfterAndBeforeExecute()
        {
            try
            {

                Clean();
                using (var context = new ModelAndContext.EntityContext())
                {
                    context.EntitySimples.Add(new EntitySimple() { ColumnInt = 10 });
                    context.EntitySimples.Add(new EntitySimple() { ColumnInt = 20 });
                    context.EntitySimples.Add(new EntitySimple() { ColumnInt = 30 });
                    context.SaveChanges();
                }
                using (var context = new ModelAndContext.EntityContext())
                {
                    var tt = context.EntitySimples.Where(x => x.ColumnInt == 10).Future();
                    var t = context.EntitySimples.Where(x => x.ColumnInt == 20).Future();

                    QueryFutureManager.OnBatchExecuting = (x) => { x.CommandText = x.CommandText.Replace("10", "30"); };

                    Assert.AreEqual(30, tt.ToList().Single().ColumnInt);

                }
                QueryFutureManager.OnBatchExecuting = null;
                using (var context = new ModelAndContext.EntityContext())
                {
                    var tt = context.EntitySimples.Where(x => x.ColumnInt == 10).Future();
                    var t = context.EntitySimples.Where(x => x.ColumnInt == 20).Future();

                    QueryFutureManager.OnBatchExecuting = (x) => { x.CommandText = x.CommandText.Replace("10", "30"); };
                    tt.ToListAsync().Wait();
                    Assert.AreEqual(30, tt.ToListAsync().Result.Single().ColumnInt);

                }
                QueryFutureManager.OnBatchExecuting = null;

                using (var context = new ModelAndContext.EntityContext())
                {
                    var tt = context.EntitySimples.Where(x => x.ColumnInt == 10).Future();
                    var t = context.EntitySimples.Where(x => x.ColumnInt == 20).Future();

                    QueryFutureManager.OnBatchExecuted = (x) => { throw new Exception("tada"); };

                    try
                    {
                        tt.ToList();
                    }
                    catch (Exception ex) { }

                    Assert.AreEqual(10, tt.ToList().Single().ColumnInt);

                }
                using (var context = new ModelAndContext.EntityContext())
                {
                    var tt = context.EntitySimples.Where(x => x.ColumnInt == 10).Future();
                    var t = context.EntitySimples.Where(x => x.ColumnInt == 20).Future();

                    QueryFutureManager.OnBatchExecuted = (x) =>
                    
                    { 
                        throw new Exception("tada"); 
                    }
                    
                    ;

                    try
                    {
                        tt.ToListAsync().Wait();
                    }
                    catch (Exception ex) { }

                    Assert.AreEqual(10, tt.ToListAsync().Result.Single().ColumnInt);

                }

            }
            finally
            {
				QueryFutureManager.OnBatchExecuted = null;
                QueryFutureManager.OnBatchExecuting = null;
            }

        }

    }
}
#endif