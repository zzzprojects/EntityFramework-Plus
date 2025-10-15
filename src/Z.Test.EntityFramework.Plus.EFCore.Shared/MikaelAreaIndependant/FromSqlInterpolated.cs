using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus.EFCore.Shared.MikaelAreaIndependant
{
#if !EFCORE_2X && !INMEMORY

    [TestClass]
	public class FromSqlInterpolated
	{ 
		[TestMethod()]
		public void FromSqlInterpolated_1()
        {
            // just test if that run!

	        // CLEAN  
	        using (var context = new ModelAndContext.EntityContext())
	        {
		        context.EntitySimples.RemoveRange(context.EntitySimples);

		        context.EntitySimpleChilds.RemoveRange(context.EntitySimpleChilds);
		        context.SaveChanges();
	        }
            using (var context = new ModelAndContext.EntityContext())
            {
                for (int i = 0; i < 5; i++)
                {
                    context.EntitySimples.Add(new EntitySimple { ColumnInt = i, EntitySimpleChild = new List<EntitySimpleChild>() { new EntitySimpleChild() { ColumnInt = i } } });
                }

                context.SaveChanges();
            } 


            // TEST  
            using (var context = new ModelAndContext.EntityContext())
            {
                var expression = context.EntitySimples.FromSqlInterpolated($"SELECT * FROM EntitySimples").Include(x => x.EntitySimpleChild).Where(x => x.ColumnInt > 2).Future();


                System.Linq.IQueryable<EntitySimple> expressiontest = context.EntitySimples
                    .FromSqlInterpolated($"SELECT * FROM EntitySimples where ColumnInt > {2}");


                var future1 = context.EntitySimples.FromSqlInterpolated($"SELECT * FROM EntitySimples").Where(x => x.ColumnInt > 2).Future();
                var list = context.EntitySimples.FromSqlInterpolated($"SELECT * FROM EntitySimples").Where(x => x.ColumnInt < 2).Future().ToList();
                int a = 1;
                var future2 = context.EntitySimples.FromSqlInterpolated($"SELECT * FROM EntitySimples where ColumnInt > ({4} - {1} - {a})").Future(); // {2}
                var list2 = context.EntitySimples.FromSqlInterpolated($"SELECT * FROM EntitySimples where ColumnInt < ({3} - {1})").Future().ToList();

                Assert.AreEqual(future1.ToList().Count, future2.ToList().Count());
                Assert.AreEqual(list2.ToList().Count, list.ToList().Count());
                //FormattableString

                var test = expression.ToList();
                var c = context.EntitySimples.FromSqlInterpolated($"SELECT * FROM EntitySimples")
                    .Include(x => x.EntitySimpleChild).Where(x => x.ColumnInt > 2).ToList();
            }

            using (var context = new ModelAndContext.EntityContext())
            {
                var expression = context.EntitySimples.FromSqlInterpolated($"SELECT * FROM EntitySimples").Include(x => x.EntitySimpleChild).Where(x => x.ColumnInt > 2).Future();
                var future1 = context.EntitySimples.FromSqlInterpolated($"SELECT * FROM EntitySimples").Where(x => x.ColumnInt > 2).Future();
                var list = context.EntitySimples.FromSqlInterpolated($"SELECT * FROM EntitySimples").Where(x => x.ColumnInt < 2).Future().ToList();

                var test = expression.ToList();
                var c = context.EntitySimples.FromSqlInterpolated($"SELECT * FROM EntitySimples").AsNoTracking()
                    .IncludeFilter(x => x.EntitySimpleChild.Where(y => y.ColumnInt == 4)).Where(x => x.ColumnInt > 2).ToList();


                var hc = context.EntitySimples.AsNoTracking()
                    .IncludeFilter(x => x.EntitySimpleChild.Where(y => y.ColumnInt == 4)).Where(x => x.ColumnInt > 2).ToList();

                var allo = context.EntitySimples.FromSqlInterpolated($"SELECT * FROM EntitySimples").AsNoTracking()
                    .Include(x => x.EntitySimpleChild).Where(x => x.ColumnInt > 2).ToList();  
            }
        }
    }
#endif
}
