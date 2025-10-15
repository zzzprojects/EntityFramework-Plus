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
					context.EntitySimples.Add(new ModelAndContext.EntitySimple { ColumInt = i, ColumString = "test" });
				}

				context.SaveChanges();
			}

			// TEST  
			using (var context = new ModelAndContext.EntityContext())
			{
				var list = context.SetDynamic("EntitySimple").AsNoTracking().ToList();

				Assert.AreEqual(3, list.Count());
				Assert.AreEqual(1, context.SetDynamic("EntitySimple", StringComparison.CurrentCulture).Where(x => ((ModelAndContext.EntitySimple)(x)).ColumInt == 2).ToList().Count());
			}

		}
	}
}
