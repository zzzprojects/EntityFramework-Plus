using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus.EFCore.Shared.MikaelAreaIndependant
{
	[TestClass]
	public class Audit_IsKey
	{
		public static void Clean()
		{
			using (var context = new ModelAndContext.EntityContext())
			{ 
				context.EntitySimples.RemoveRange(context.EntitySimples);
				context.SaveChanges();
			}
		}

		[TestMethod]
		public void Test_IsKey()
		{
			var factory = AuditManager.DefaultConfiguration.AutoSavePreAction;
			AuditManager.DefaultConfiguration.AutoSavePreAction = null;
			try
			{

			Clean();
				using (var context = new ModelAndContext.EntityContext())
				{
					var entity = new EntitySimple();
					var audit1 = new Audit();
					var audit2 = new Audit();
					var audit3 = new Audit();


					context.EntitySimples.Add(entity);
					context.SaveChanges(audit1);


					entity.ColumnString = "Update";
					context.SaveChanges(audit2);


					context.EntitySimples.Remove(entity);
					context.SaveChanges(audit3);
					Assert.AreEqual(audit1.Entries.First().EntitySetName, "EntitySimples");
					Assert.IsTrue(audit1.Entries.First().Properties.Where(x => x.PropertyName == "ID").First().IsKey);
					Assert.IsTrue(audit2.Entries.First().Properties.Where(x => x.PropertyName == "ID").First().IsKey);
					Assert.IsTrue(audit3.Entries.First().Properties.Where(x => x.PropertyName == "ID").First().IsKey);


					Assert.IsTrue(audit1.Entries.First().Properties.Where(x => x.PropertyName != "ID").All(x => !x.IsKey));
					Assert.IsTrue(audit2.Entries.First().Properties.Where(x => x.PropertyName != "ID").All(x => !x.IsKey));
					Assert.IsTrue(audit3.Entries.First().Properties.Where(x => x.PropertyName != "ID").All(x => !x.IsKey));

				}
			}
			finally
			{
				AuditManager.DefaultConfiguration.AutoSavePreAction = factory;
			}


		}
	}
}
