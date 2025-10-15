using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus.EFCore.Shared.MikaelAreaIndependant
{
	[TestClass]
	public class ConverterFutur
	{
		[TestMethod()]
		public void ConverterFutur_1()
		{
			Clean(); 
			var GuidValue = GenerateGuid();
			Seed(GuidValue);

			using (var context = new ModelAndContext.EntityContext())
			{ 
				var bonresult = context.EntitySimpleHasConverterSimples.AsNoTracking().Where(x =>  x.ColumnGuid == GuidValue).Count();
				var checkfinal = context.EntitySimpleHasConverterSimples.Where(x => x.ColumnGuid == GuidValue).DeferredCount().FutureValue();
				var t = context.EntitySimpleHasConverterSimples.Take(1).Future();
				var t2 = context.EntitySimpleHasConverterSimples.Take(2).Future();
				
				var c1 = t2.ToList();

				Assert.AreEqual(bonresult, checkfinal.Value);
			}
		}

		private static string GenerateGuid()
		{
			return Guid.NewGuid().ToString("N").ToUpperInvariant();
		}
		public static void Clean()
		{
			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimpleHasConverterSimples.RemoveRange(context.EntitySimpleHasConverterSimples);
				context.SaveChanges();
			}
		}

		public static void Seed(string guidValue)
		{
			// SEED  
			using (var context = new ModelAndContext.EntityContext())
			{
				for (int i = 0; i < 3; i++)
				{
					context.EntitySimpleHasConverterSimples.Add(new EntitySimpleHasConverterSimple { ColumnGuid = guidValue });
				}

				context.SaveChanges();
			}

			//// TEST  
			//using (var context = new ModelAndContext.EntityContext())
			//{
			//	List<EntitySimpleHasConverterSimple> list = new List<EntitySimpleHasConverterSimple>();
			//	list.Add(new EntitySimpleHasConverterSimple {   ColumnGuid = guidValue });
			//	list.Add(new EntitySimpleHasConverterSimple {   ColumnGuid = guidValue });
			//	list.Add(new EntitySimpleHasConverterSimple {   ColumnGuid = guidValue });

			//	context.Add(list);
			//}
		} 
	}
}
