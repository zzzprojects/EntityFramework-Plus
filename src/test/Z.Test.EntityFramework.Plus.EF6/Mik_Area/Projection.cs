using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus.Mik_Area
{ 

	[TestClass]
    public class Projection
    {
		public static void cleannup()
		{
			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimpleWithChilds.RemoveRange(context.EntitySimpleWithChilds);
				context.EntitySimpleChilds.RemoveRange(context.EntitySimpleChilds); 
				context.SaveChanges();
			}
		}

		[TestMethod()]
		public void Projection_01()
		{
			cleannup();

			// SEED  
			using (var context = new ModelAndContext.EntityContext())
			{
				for (int i = 0; i < 3; i++)
				{
					context.EntitySimpleWithChilds.Add(new ModelAndContext.EntitySimpleWithChild { ColumnInt = i, ColumnString = "test", EntitySimpleChild = new ModelAndContext.EntitySimpleChild() {ColumnInt = 100+ i } });
				}

				context.SaveChanges();
			}

			// TEST  
			using (var context = new ModelAndContext.EntityContext())
			{
				var list = context.EntitySimpleWithChilds.Project(x => new {x.ColumnInt });
				Assert.AreEqual(3, list.Count);
				foreach( var entity in context.EntitySimpleWithChilds.ToList())
                {
					Assert.IsTrue(list.Where(x => x.ColumnInt == entity.ColumnInt).Count() == 1);
                }
			}

		}

		[TestMethod()]
		public void Projection_02()
		{
			cleannup();

			// SEED  
			using (var context = new ModelAndContext.EntityContext())
			{
				for (int i = 0; i < 3; i++)
				{
					context.EntitySimpleWithChilds.Add(new ModelAndContext.EntitySimpleWithChild {IsActive = true, ColumnInt = i, ColumnString = "test", EntitySimpleChild = new ModelAndContext.EntitySimpleChild() { ColumnInt = 100 + i } });
				}
				for (int i = 0; i < 3; i++)
				{
					context.EntitySimpleWithChilds.Add(new ModelAndContext.EntitySimpleWithChild { IsActive = false, ColumnInt = i, ColumnString = "test", EntitySimpleChild = new ModelAndContext.EntitySimpleChild() { ColumnInt = 100 + i } });
				}

				context.SaveChanges();
			}

			// TEST  
			using (var context = new ModelAndContext.EntityContext())
			{
				var list = context.EntitySimpleWithChilds.Include("EntitySimpleChild").Where(x => x.IsActive).Project(x => new {
					ID = x.ID,
					ColumnInt = x.ColumnInt + 10,
					ColumnString = x.ColumnString,
					EntitySimpleChild = new
					{
						ColumnInt = x.EntitySimpleChild.ColumnInt,
						ColumnString = "Test",
						IsActive = x.ColumnInt == 2
					}
				});
				Assert.AreEqual(3, list.Count);
				foreach (var entity in context.EntitySimpleWithChilds.Include("EntitySimpleChild").Where(x => x.IsActive).ToList())
				{
					var entityProjected = list.Where(x => x.ID == entity.ID).Single();

					Assert.AreEqual(entity.ColumnInt + 10, entityProjected.ColumnInt);
					Assert.AreEqual(entity.ColumnString, entityProjected.ColumnString);
					Assert.AreEqual(entity.EntitySimpleChild.ColumnInt , entityProjected.EntitySimpleChild.ColumnInt);
					Assert.AreEqual("Test", entityProjected.EntitySimpleChild.ColumnString);
					Assert.AreEqual(entity.ColumnInt == 2, entityProjected.EntitySimpleChild.IsActive);
				}
			}

		}
	}
}
