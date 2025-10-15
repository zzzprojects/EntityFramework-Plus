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
	public class DynamicSet
	{
		[TestMethod()]
		public void DynamicSet_01()
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
					context.EntitySimples.Add(new EntitySimple { ColumnInt = i, ColumnString = "test" });
				}

				context.SaveChanges();
			}

			// TEST  
			using (var context = new ModelAndContext.EntityContext())
			{
				var list = context.SetDynamic("EntitySimple").ToList();

				Assert.AreEqual(3, list.Count());
				Assert.AreEqual(1, context.SetDynamic("EntitySimple", StringComparison.CurrentCulture).Where(x => ((EntitySimple)(x)).ColumnInt == 2).ToList().Count());
			}

		}
	}
}
