#if !EFCORE_2X && !INMEMORY
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus.EFCore.Shared.MikaelAreaIndependant
{
	[TestClass]
	public class IncludeOptimizedWithFutur
	{
		public static void Clean()
		{
			using (var context = new ModelAndContext.EntityContext())
			{
				context.RemoveRange(context.EntitySimples);
				context.RemoveRange(context.EntitySimpleChilds); 
				context.SaveChanges();
			}
		}

		[TestMethod]
		public void IncludeOptimizedWithFutur_01()
		{
			Clean();
			using (var context = new ModelAndContext.EntityContext())
			{
				for (int i = 0; i < 3; i++)
				{
					context.EntitySimples.Add(new EntitySimple { ColumnInt = i, EntitySimpleChild = new List<EntitySimpleChild>() { new EntitySimpleChild() { ColumnInt = i }, new EntitySimpleChild() { ColumnInt = i } } });
				} 

				context.SaveChanges();
			}

			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimpleChilds.DeleteFromQuery();
				var list = context.EntitySimples.IncludeOptimized(x => x.EntitySimpleChild).Future();

				var resolve = context.EntitySimples.IncludeOptimized(x => x.EntitySimpleChild).Future(); 
				var countvalue = context.EntitySimples.DeferredCount().FutureValue<int>();
				var value = countvalue.Value;
				context.EntitySimples.DeleteFromQuery();

				var list2 = resolve.ToList();
				var list3 = list.ToList();

				Assert.AreEqual(3, value);
				Assert.IsNotNull(list2);
				Assert.IsNotNull(list3);
				Assert.IsTrue(list2.All(x => x.EntitySimpleChild != null));
				Assert.IsTrue(list3.All(x => x.EntitySimpleChild != null)); 
			}
		}

		[TestMethod]
		public void IncludeOptimizedWithFutur_02()
		{
			Clean();
			using (var context = new ModelAndContext.EntityContext())
			{
				for (int i = 0; i < 3; i++)
				{
					context.EntitySimples.Add(new EntitySimple { ColumnInt = i, EntitySimpleChild = new List<EntitySimpleChild>() { new EntitySimpleChild() { ColumnInt = i }, new EntitySimpleChild() { ColumnInt = i } } });
				}

				context.SaveChanges();
			}

			using (var context = new ModelAndContext.EntityContext())
			{ 
				var list = context.EntitySimples.IncludeOptimized(x => x.EntitySimpleChild).Future();

				var resolve = context.EntitySimples.IncludeOptimized(x => x.EntitySimpleChild).Future();
				var countvalue = context.EntitySimples.DeferredCount().FutureValue<int>();
				var value = countvalue.Value;
				context.EntitySimpleChilds.DeleteFromQuery();
				context.EntitySimples.DeleteFromQuery();

				var list2 = resolve.ToList();
				var list3 = list.ToList();

				Assert.AreEqual(3, value);
				Assert.IsNotNull(list2);
				Assert.IsNotNull(list3);
				Assert.IsTrue(list2.All(x => x.EntitySimpleChild != null));
				Assert.IsTrue(list3.All(x => x.EntitySimpleChild != null));
				Assert.AreEqual(6 ,list2.Sum(x => x.EntitySimpleChild.Count()));
				Assert.AreEqual(6 ,list3.Sum(x => x.EntitySimpleChild.Count()));
			}
		}
	}
}
#endif