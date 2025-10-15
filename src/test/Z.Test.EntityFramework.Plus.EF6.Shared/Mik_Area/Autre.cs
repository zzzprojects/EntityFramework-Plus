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

                        context.EntitySimples.Add(new ModelAndContext.EntitySimple() { Id = 800000, ColumInt = 10 }); 
                        context.SaveChanges();
                        context.EntitySimples.InsertFromQuery(x => new ModelAndContext.EntitySimple() { Id = 800000, ColumInt = 10 });
                    }
                    catch (Exception e)
                    { 
                        context.ChangeTracker.Entries().ToList().ForEach(x => x.State = EntityState.Unchanged);
                        context.EntitySimples.SqlServerSetIdentityInsertOn(); 
                        context.EntitySimples.InsertFromQuery(x => new ModelAndContext.EntitySimple() { Id = 800001, ColumInt = 10 });
                        context.EntitySimples.Where(x => x.Id != 800001).DeleteFromQuery(); 
                        context.EntitySimples.SqlServerSetIdentityInsertOff();
                    }
                }

                using (var context = new ModelAndContext.EntityContext())
                {
                    var t = context.EntitySimples.Single();

                    Assert.AreEqual(800001, t.Id);
                }
                Clean();
                using (var context = new ModelAndContext.EntityContext())
                {
                    try
                    {

                        context.EntitySimples.Add(new ModelAndContext.EntitySimple() { Id = 800000, ColumInt = 10 });
                        context.SaveChanges();
                        context.EntitySimples.InsertFromQuery(x => new ModelAndContext.EntitySimple() { Id = 800001, ColumInt = 10 });
                    }
                    catch (Exception e)
                    {
                        context.ChangeTracker.Entries().ToList().ForEach(x => x.State = EntityState.Unchanged);
                        context.SqlServerSetIdentityInsertOn<ModelAndContext.EntitySimple>();
                        context.EntitySimples.InsertFromQuery(x => new ModelAndContext.EntitySimple() { Id = 800001, ColumInt = 10 });
                        context.EntitySimples.Where(x => x.Id != 800001).DeleteFromQuery();
                        context.SqlServerSetIdentityInsertOff<ModelAndContext.EntitySimple>();
                    }
                }

                using (var context = new ModelAndContext.EntityContext())
                {
                    var t = context.EntitySimples.Single();

                    Assert.AreEqual(800001, t.Id);
                }
                Clean();
                using (var context = new ModelAndContext.EntityContext())
                {
                    try
                    {

                        context.EntitySimples.Add(new ModelAndContext.EntitySimple() { Id = 800000, ColumInt = 10 });
                        context.SaveChanges();
                        context.EntitySimples.InsertFromQuery(x => new ModelAndContext.EntitySimple() { Id = 800001, ColumInt = 10 });
                    }
                    catch (Exception e)
                    {
                        context.ChangeTracker.Entries().ToList().ForEach(x => x.State = EntityState.Unchanged);
                        context.SqlServerSetIdentityInsertOn(typeof(ModelAndContext.EntitySimple));
                        context.EntitySimples.InsertFromQuery(x => new ModelAndContext.EntitySimple() { Id = 800001, ColumInt = 10 });
                        context.EntitySimples.Where(x => x.Id != 800001).DeleteFromQuery();
                        context.SqlServerSetIdentityInsertOff(typeof(ModelAndContext.EntitySimple));
                    }
                }

                using (var context = new ModelAndContext.EntityContext())
                {
                    var t = context.EntitySimples.Single();

                    Assert.AreEqual(800001, t.Id);
                }
                using (var context = new ModelAndContext.EntityContext())
                {
                    bool hasError = false;
                    try
                    {

                        context.EntitySimples.Add(new ModelAndContext.EntitySimple() { Id = 800000, ColumInt = 10 });
                        context.SaveChanges();
                        context.SqlServerSetIdentityInsertOn(typeof(ModelAndContext.EntitySimple));
                        context.SqlServerSetIdentityInsertOff(typeof(ModelAndContext.EntitySimple));
                        context.EntitySimples.InsertFromQuery(x => new ModelAndContext.EntitySimple() { Id = 800001, ColumInt = 10 });
                        context.EntitySimples.Where(x => x.Id != 800001).DeleteFromQuery();
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
                //using (var context = new ModelAndContext.EntityContext())
                //{
                //    context.EntitySimples.SqlServerSetIdentityInsertOff();
                //}
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
                    context.EntitySimples.Add(new ModelAndContext.EntitySimple() { ColumInt = 10 });
                    context.EntitySimples.Add(new ModelAndContext.EntitySimple() { ColumInt = 20 });
                    context.EntitySimples.Add(new ModelAndContext.EntitySimple() { ColumInt = 30 });
                    context.SaveChanges();
                }
                using (var context = new ModelAndContext.EntityContext())
                {
                    var tt = context.EntitySimples.Where(x => x.ColumInt == 10).Future();
                    var t = context.EntitySimples.Where(x => x.ColumInt == 20).Future();

                    QueryFutureManager.OnBatchExecuting = (x) => { x.CommandText = x.CommandText.Replace("10", "30"); };

                    Assert.AreEqual(30, tt.ToList().Single().ColumInt);

                }
                QueryFutureManager.OnBatchExecuting = null;
                using (var context = new ModelAndContext.EntityContext())
                {
                    var tt = context.EntitySimples.Where(x => x.ColumInt == 10).Future();
                    var t = context.EntitySimples.Where(x => x.ColumInt == 20).Future();

                    QueryFutureManager.OnBatchExecuting = (x) => { x.CommandText = x.CommandText.Replace("10", "30"); };
                    tt.ToListAsync().Wait();
                    Assert.AreEqual(30, tt.ToListAsync().Result.Single().ColumInt);

                }
                QueryFutureManager.OnBatchExecuting = null;

                using (var context = new ModelAndContext.EntityContext())
                {
                    var tt = context.EntitySimples.Where(x => x.ColumInt == 10).Future();
                    var t = context.EntitySimples.Where(x => x.ColumInt == 20).Future();

                    QueryFutureManager.OnBatchExecuted = (x) => { throw new Exception("tada"); };

                    try
                    {
                        tt.ToList();
                    }
                    catch (Exception ex) { }

                    Assert.AreEqual(10, tt.ToList().Single().ColumInt);

                }
                using (var context = new ModelAndContext.EntityContext())
                {
                    var tt = context.EntitySimples.Where(x => x.ColumInt == 10).Future();
                    var t = context.EntitySimples.Where(x => x.ColumInt == 20).Future();

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

                    Assert.AreEqual(10, tt.ToListAsync().Result.Single().ColumInt);

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