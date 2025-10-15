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
    public class Cas_TestIntercepter
    {
        [TestMethod]
        public void Cas_TestIntercepter_1()
        {
            try
            {
                ModelAndContext.test = 0;
                ModelAndContext.actifInterceptor = true;
                var checkTrue = " IsAsyncé: True";
                var checkFalse = " IsAsyncé: False";
                using (var context = new ModelAndContext.EntityContext())
                {
                    context.EntitySimples.RemoveRange(context.EntitySimples);
                    var entity = new ModelAndContext.EntitySimple();
                    entity.ColumInt = 125;
                    entity.ColumIntNullable = 1250;

                    context.EntitySimples.Add(entity);
                    context.SaveChanges();

                    var sql = "";
                    context.Database.Log = s => sql += s;
                    string a = "";
                    string b = "";
                    var t = context.EntitySimples.Where(x => x.ColumInt == 4 && a == b ).Future();

                    var ta = context.EntitySimples.Where(x => x.ColumInt == 44 && a == b).Future();

                    var asda = ta.ToList();
                    Assert.AreEqual(0, ModelAndContext.test);

                    Assert.IsTrue(sql.Contains(checkFalse));

                    Assert.IsFalse(sql.Contains(checkTrue));

                    sql = "";

                    var tasd = context.EntitySimples.Where(x => x.ColumInt == 4666 && a == b).Future();

                    var tsaa = context.EntitySimples.Where(x => x.ColumInt == 4452 && a == b).Future();



                    var asadsda = tsaa.ToListAsync();
                    Assert.AreEqual(5,ModelAndContext.test);

                    Assert.AreEqual(6, sql.Split('é').Count());
                    Assert.IsTrue(sql.Contains(checkTrue));

                    Assert.IsFalse(sql.Contains(checkFalse));
                }
            }
            finally
            {
                ModelAndContext.test = 0;
                ModelAndContext.actifInterceptor = false;
            }

        }
        [TestMethod]
        public void Cas_TestIntercepter_2()
        {
            try
            {
                ModelAndContext.test = 0;
                ModelAndContext.actifInterceptor = true;
                var checkTrue = " IsAsyncé: True";
                var checkFalse = " IsAsyncé: False";
                using (var context = new ModelAndContext.EntityContext())
                {
                    context.EntitySimples.RemoveRange(context.EntitySimples);
                    var entity = new ModelAndContext.EntitySimple();
                    entity.ColumInt = 125;
                    entity.ColumIntNullable = 1250;

                    context.EntitySimples.Add(entity);
                    context.SaveChanges();

                    var sql = "";
                    context.Database.Log = s => sql += s;
                    string a = "";
                    string b = "";

                    var t = context.EntitySimples.Where(x => x.ColumInt == 4 && a == b).Select(c => c.ColumInt)

                    .DefaultIfEmpty()

                    .DeferredMin()

                    .FromCacheAsync().Result;

                    Assert.AreEqual(4, ModelAndContext.test);

                    Assert.IsTrue(sql.Contains(checkTrue));

                    Assert.IsFalse(sql.Contains(checkFalse)); 
                }
            }
            finally
            {
                ModelAndContext.test = 0;
                ModelAndContext.actifInterceptor = false;
            }

        }
        [TestMethod]
        public void Cas_TestIntercepter_3()
        {
            try
            {
                ModelAndContext.test = 0;
                ModelAndContext.actifInterceptor = true;
                var checkTrue = " IsAsyncé: True";
                var checkFalse = " IsAsyncé: False";
                using (var context = new ModelAndContext.EntityContext())
                {
                    context.EntitySimples.RemoveRange(context.EntitySimples);
                    var entity = new ModelAndContext.EntitySimple();
                    entity.ColumInt = 125;
                    entity.ColumIntNullable = 1250;

                    context.EntitySimples.Add(entity);
                    context.SaveChanges();

                    var sql = "";
                    context.Database.Log = s => sql += s;
                    string a = "";
                    string b = "";

                    var t = context.EntitySimples.Where(x => x.ColumInt == 4 && a == b).Select(c => c.ColumInt)

                    .DefaultIfEmpty()

                    .DeferredMin()

                    .FromCache();

                    Assert.AreEqual(0, ModelAndContext.test);

                    Assert.IsTrue(sql.Contains(checkFalse));

                    Assert.IsFalse(sql.Contains(checkTrue));
                }
            }
            finally
            {
                ModelAndContext.test = 0;
                ModelAndContext.actifInterceptor = false;
            }

        }
    }
}
