using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;
using Z.Test.EntityFramework.Plus.Mik_Area;

namespace Z.Test.EntityFramework.Plus.EFCore.Shared.MikaelAreaIndependant
{
	[TestClass]
	public class UpdateFromQueryDic
	{
		[TestMethod()]
		public void Update_1()
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
				context.EntitySimples.Update(new Dictionary<string, object>() { { "ColumString", "Update" } });

				Assert.AreEqual(3, context.EntitySimples.Where(x => x.ColumString == "Update").Count());
			}
		}
	}
}