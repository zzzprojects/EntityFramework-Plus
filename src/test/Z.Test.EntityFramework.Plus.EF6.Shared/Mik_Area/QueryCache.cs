using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus.Mik_Area
{
	[TestClass]
	public class QueryCache
	{

		[TestMethod()]
		public void QueryCache_1()
		{
			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimples.RemoveRange(context.EntitySimples);

				context.SaveChanges();
			}


			// SEED  
			using (var context = new ModelAndContext.EntityContext())
			{
				for (int i = 0; i < 3; i++)
				{
					context.EntitySimples.Add(new ModelAndContext.EntitySimple { ColumInt = i, ColumString = "test" });
				}

				context.SaveChanges();
			}

			// TEST  
			using (var context = new ModelAndContext.EntityContext())
			{
				var c = context.EntitySimples.AsNoTracking().Where(x => x.ColumString == "test").FromCache().ToList();
				var lc = context.EntitySimples.AsNoTracking().Where(x => x.ColumString == "test").FromCache("teast").ToList();
				var llc = context.EntitySimples.AsNoTracking().Where(x => x.ColumString == "test").FromCache().ToList();

				Assert.AreEqual(3, lc.Count);
				Assert.AreEqual(3, c.Count);
				Assert.AreEqual(3, llc.Count);
			}

			// TEST  
			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimples.Where(x => x.ColumString == "test").Take(1).Delete();
				var c = context.EntitySimples.AsNoTracking().Where(x => x.ColumString == "test").FromCache().ToList();


				QueryCacheManager.ExpireAll();

				var bc = context.EntitySimples.AsNoTracking().Where(x => x.ColumString == "test").FromCache().ToList();
				var jjc = context.EntitySimples.AsNoTracking().Where(x => x.ColumString == "test").FromCache("teast").ToList();
				var jjdc = context.EntitySimples.AsNoTracking().Where(x => x.ColumString == "test").FromCache("test").ToList();

				Assert.AreEqual(2, bc.Count);
				Assert.AreEqual(2, jjc.Count);
				Assert.AreEqual(2, jjdc.Count);
				Assert.AreEqual(3, c.Count);
			}

			// TEST  
			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimples.Where(x => x.ColumString == "test").Take(1).Delete();
				var c = context.EntitySimples.AsNoTracking().Where(x => x.ColumString == "test").FromCache("test").ToList();

				var dc = context.EntitySimples.AsNoTracking().Where(x => x.ColumString == "test").FromCache().ToList();

				QueryCacheManager.ExpireType(typeof(ModelAndContext.EntitySimple));

				var bc = context.EntitySimples.AsNoTracking().Where(x => x.ColumString == "test").FromCache().ToList();
				var bac = context.EntitySimples.AsNoTracking().Where(x => x.ColumString == "test").FromCache("test").ToList();

				Assert.AreEqual(1, bc.Count);
				Assert.AreEqual(1, bac.Count);
				Assert.AreEqual(2, c.Count);
				Assert.AreEqual(2, dc.Count);
			}

			// TEST  
			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimples.Where(x => x.ColumString == "test").Take(1).Delete();
				var c = context.EntitySimples.AsNoTracking().Where(x => x.ColumString == "test").FromCache("test").ToList();

				var dc = context.EntitySimples.AsNoTracking().Where(x => x.ColumString == "test").FromCache().ToList();

				context.EntitySimples.ExpireCache();

				var bc = context.EntitySimples.AsNoTracking().Where(x => x.ColumString == "test").FromCache().ToList();
				var bac = context.EntitySimples.AsNoTracking().Where(x => x.ColumString == "test").FromCache("test").ToList();

				Assert.AreEqual(0, bc.Count);
				Assert.AreEqual(0, bac.Count);
				Assert.AreEqual(1, c.Count);
				Assert.AreEqual(1, dc.Count);
			}
		}
	}
}
