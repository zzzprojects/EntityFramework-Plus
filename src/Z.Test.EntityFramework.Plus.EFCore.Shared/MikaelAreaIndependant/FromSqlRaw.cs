using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

#if !EFCORE_2X && !INMEMORY
using Microsoft.Data.SqlClient;
#endif

namespace Z.Test.EntityFramework.Plus.EFCore.Shared.MikaelAreaIndependant
{
#if !EFCORE_2X && !INMEMORY

	[TestClass]
	public class FromSqlRaw
	{
		[TestMethod()]
		public void QueryCache_1()
		{
			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimples.RemoveRange(context.EntitySimples);
				context.EntitySimpleChilds.RemoveRange(context.EntitySimpleChilds);

				context.SaveChanges();
			}


			// SEED  
			using (var context = new ModelAndContext.EntityContext())
			{
				for (int i = 0; i < 3; i++)
				{
					context.EntitySimples.Add(new EntitySimple { ColumnInt = i, ColumnString = "test" });
				}

				context.SaveChanges();
			}

			// TEST  
			using (var context = new ModelAndContext.EntityContext())
			{
				var columnInt = new SqlParameter("ColumnInt", 1);
				var entity1 = context.EntitySimples.FromSqlRaw("Select * from EntitySimples Where ColumnInt = @ColumnInt", columnInt).FromCache().First();
				columnInt = new SqlParameter("ColumnInt", 2);
				var entity2 = context.EntitySimples.FromSqlRaw("Select * from EntitySimples Where ColumnInt = @ColumnInt", columnInt).FromCache().First();
 
				Assert.AreEqual(2, entity2.ColumnInt);
				Assert.AreEqual(1, entity1.ColumnInt);
			}
			 
		}
	}
#endif
}
