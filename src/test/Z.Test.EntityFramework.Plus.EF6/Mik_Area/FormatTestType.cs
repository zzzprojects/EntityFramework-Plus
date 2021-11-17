using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;
using static Z.Test.EntityFramework.Plus.Mik_Area.ModelAndContext;

namespace Z.Test.EntityFramework.Plus.Mik_Area
{
    [TestClass]
    public class FormatTestType
    {
		public static void Clean()
		{
			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimples.RemoveRange(context.EntitySimples);
				context.AuditEntries.RemoveRange(context.AuditEntries);
				context.SaveChanges();
			}
		}

		[TestMethod]
		public void Test_1()
		{
			Clean();

			var entity = new EntitySimple();
			var old = new List<Func<object, string, object, Func<object, object>>>(AuditManager.DefaultConfiguration.EntityValueFormatters);  
			try
			{
				AuditManager.DefaultConfiguration.FormatType<int?>(x => x.HasValue ? x + 500 : -10);
				using (var context = new ModelAndContext.EntityContext())
				{
					entity.ColumInt = 125;
					entity.ColumIntNullable = 1250;

					context.EntitySimples.Add(entity);

					var audit = new Audit();
					audit.CreatedBy = "Someone";
					context.SaveChanges(audit);
					context.AuditEntries.AddRange(audit.Entries);

					context.SaveChanges();


				}

				using (var context = new ModelAndContext.EntityContext())
				{ 
					var entityEFplus = context.AuditEntries.IncludeOptimized(x => x.Properties).First();

					Assert.AreEqual((125 + 500).ToString(), entityEFplus.Properties.Where(x => x.PropertyName == "ColumInt").First().NewValueFormatted);
					Assert.AreEqual((1250 + 500).ToString(), entityEFplus.Properties.Where(x => x.PropertyName == "ColumIntNullable").First().NewValueFormatted);
				} 

			}
			finally
			{
				AuditManager.DefaultConfiguration.EntityValueFormatters = old;
				AuditManager.DefaultConfiguration.ValueFormatterDictionary = new System.Collections.Concurrent.ConcurrentDictionary<string, Func<object, object>>();
			}

		}
		[TestMethod]
		public void Test_2()
		{
			Clean();

			var entity = new EntitySimple();
			var old = new List<Func<object, string, object, Func<object, object>>>(AuditManager.DefaultConfiguration.EntityValueFormatters);
			try
			{
				AuditManager.DefaultConfiguration.FormatType<int?>(x => x.HasValue ? x + 500 : -10);
				using (var context = new ModelAndContext.EntityContext())
				{
					entity.ColumInt = 125;
					entity.ColumIntNullable = null;

					context.EntitySimples.Add(entity);

					var audit = new Audit();
					audit.CreatedBy = "Someone";
					context.SaveChanges(audit);

					context.AuditEntries.AddRange(audit.Entries);

					context.SaveChanges();

				}

				using (var context = new ModelAndContext.EntityContext())
				{
					var entityEFplus = context.AuditEntries.IncludeOptimized(x => x.Properties).First();

					Assert.AreEqual((125 + 500).ToString(),   entityEFplus.Properties.Where(x => x.PropertyName == "ColumInt").First().NewValueFormatted);
					Assert.AreEqual(null, entityEFplus.Properties.Where(x => x.PropertyName == "ColumIntNullable").First().NewValueFormatted);
				}

			}
			finally
			{
				AuditManager.DefaultConfiguration.EntityValueFormatters = old;
				AuditManager.DefaultConfiguration.ValueFormatterDictionary = new System.Collections.Concurrent.ConcurrentDictionary<string, Func<object, object>>();
			}
		}
		[TestMethod]
		public void Test_3()
		{
			Clean();

			var entity = new EntitySimple();
			var old = new List<Func<object, string, object, Func<object, object>>>(AuditManager.DefaultConfiguration.EntityValueFormatters);
			try
			{
				AuditManager.DefaultConfiguration.FormatType<int>(x => x + 500);
				using (var context = new ModelAndContext.EntityContext())
				{
					entity.ColumInt = 125;
					entity.ColumIntNullable = 1250;

					context.EntitySimples.Add(entity);

					var audit = new Audit();
					audit.CreatedBy = "Someone";
					context.SaveChanges(audit);
					context.AuditEntries.AddRange(audit.Entries);

					context.SaveChanges();


				}

				using (var context = new ModelAndContext.EntityContext())
				{
					var entityEFplus = context.AuditEntries.IncludeOptimized(x => x.Properties).First();

					Assert.AreEqual((125 + 500).ToString(), entityEFplus.Properties.Where(x => x.PropertyName == "ColumInt").First().NewValueFormatted);
					Assert.AreEqual((1250 + 500).ToString(), entityEFplus.Properties.Where(x => x.PropertyName == "ColumIntNullable").First().NewValueFormatted);
				}

			}
			finally
			{
				AuditManager.DefaultConfiguration.EntityValueFormatters = old;
				AuditManager.DefaultConfiguration.ValueFormatterDictionary = new System.Collections.Concurrent.ConcurrentDictionary<string, Func<object, object>>();
			}

		}
		[TestMethod]
		public void Test_4()
		{
			Clean();

			var entity = new EntitySimple();
			var old = new List<Func<object, string, object, Func<object, object>>>(AuditManager.DefaultConfiguration.EntityValueFormatters);
			try
			{
				AuditManager.DefaultConfiguration.FormatType<int>(x => x + 500);
				using (var context = new ModelAndContext.EntityContext())
				{
					entity.ColumInt = 125;
					entity.ColumIntNullable = null;

					context.EntitySimples.Add(entity);

					var audit = new Audit();
					audit.CreatedBy = "Someone";
					context.SaveChanges(audit);

					context.AuditEntries.AddRange(audit.Entries);

					context.SaveChanges();

				}

				using (var context = new ModelAndContext.EntityContext())
				{
					var entityEFplus = context.AuditEntries.IncludeOptimized(x => x.Properties).First();

					Assert.AreEqual((125 + 500).ToString(), entityEFplus.Properties.Where(x => x.PropertyName == "ColumInt").First().NewValueFormatted);
					Assert.AreEqual(null, entityEFplus.Properties.Where(x => x.PropertyName == "ColumIntNullable").First().NewValueFormatted);
				}

			}
			finally
			{
				AuditManager.DefaultConfiguration.EntityValueFormatters = old;
				AuditManager.DefaultConfiguration.ValueFormatterDictionary = new System.Collections.Concurrent.ConcurrentDictionary<string, Func<object, object>>();
			}
		}
	}
}
