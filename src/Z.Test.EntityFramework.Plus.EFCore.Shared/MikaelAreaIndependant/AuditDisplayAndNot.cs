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
	public class AuditDisplayAndNot
	{
		[TestClass]
		public class Audit_IsKey
		{
			public static void Clean()
			{
				using (var context = new ModelAndContext.EntityContext())
				{
					context.EntitySimples.RemoveRange(context.EntitySimples);
					context.EntitySimpleWithDisplays.RemoveRange(context.EntitySimpleWithDisplays);
					context.SaveChanges();
				}
			}

			[TestMethod]
			public void Test_1()
			{ 
				Clean();
				var entity = new EntitySimple();
				var old = AuditManager.DefaultConfiguration.AutoSavePreAction;
				try
				{

					// some stuff can be modified depending on Mikael or Jonathan context 
					AuditManager.DefaultConfiguration.AutoSavePreAction = (context, audit) =>
						(context as ModelAndContext.EntityContext).AuditEntries.AddRange(audit.Entries);

					AuditManager.DefaultConfiguration.DataAnnotationDisplayName();
					using (var context = new ModelAndContext.EntityContext())
					{
						entity.ColumnInt = 125;

						context.EntitySimples.Add(entity);

						var audit = new Audit();
						audit.CreatedBy = "Someone";
						context.SaveChanges(audit);

					}

					using (var context = new ModelAndContext.EntityContext())
					{


						var entityEFplus = context.AuditEntries.Where(entity).First();

						Assert.AreEqual(entity.ColumnInt, int.Parse(((string)entityEFplus.Properties.Where(x => x.PropertyName == "ColumnInt").First().NewValue)));
					}

					using (var context = new ModelAndContext.EntityContext())
					{


						var entityEFplus = context.AuditEntries.Where<AuditEntry, EntitySimple>(entity.ID).First();

						Assert.AreEqual(entity.ColumnInt, int.Parse(((string)entityEFplus.Properties.Where(x => x.PropertyName == "ColumnInt").First().NewValue)));
					}
				}
				finally
				{
					AuditManager.DefaultConfiguration.AutoSavePreAction  = old;
				}


			}

			[TestMethod]
			public void Test_2()
			{
				Clean();
				var entity = new EntitySimpleWithDisplay(); 
				var old = AuditManager.DefaultConfiguration.AutoSavePreAction;
				try
				{

					// some stuff can be modified depending on Mikael or Jonathan context 
					AuditManager.DefaultConfiguration.AutoSavePreAction = (context, audit) =>
						(context as ModelAndContext.EntityContext).AuditEntries.AddRange(audit.Entries);

					AuditManager.DefaultConfiguration.DataAnnotationDisplayName();
					using (var context = new ModelAndContext.EntityContext())
					{
						entity.ColumInt = 125;

						context.EntitySimpleWithDisplays.Add(entity);

						var audit = new Audit();
						audit.CreatedBy = "Someone";
						context.SaveChanges(audit);

					}

					using (var context = new ModelAndContext.EntityContext())
					{ 
						var entityEFplus = context.AuditEntries.Where(entity).First();

						Assert.AreEqual(entity.ColumInt, int.Parse(((string)entityEFplus.Properties.Where(x => x.PropertyName == "ColumInt").First().NewValue)));
					}

					using (var context = new ModelAndContext.EntityContext())
					{ 
						var entityEFplus = context.AuditEntries.Where<AuditEntry, EntitySimpleWithDisplay>(entity.Id).First();

						Assert.AreEqual(entity.ColumInt, int.Parse(((string)entityEFplus.Properties.Where(x => x.PropertyName == "ColumInt").First().NewValue)));
					}
				}
				finally
				{
					AuditManager.DefaultConfiguration.AutoSavePreAction  = old;
				}

			}

			[TestMethod]
			public void Test_3()
			{
				Clean();
				var entity = new EntitySimpleWithDisplay();
				var old = AuditManager.DefaultConfiguration.AutoSavePreAction;
				try
				{

					// some stuff can be modified depending on Mikael or Jonathan context 
					AuditManager.DefaultConfiguration.AutoSavePreAction = (context, audit) =>
						(context as ModelAndContext.EntityContext).AuditEntries.AddRange(audit.Entries);

					AuditManager.DefaultConfiguration.DataAnnotationDisplayName();
					using (var context = new ModelAndContext.EntityContext())
					{
						entity.ColumInt = 125;

						context.EntitySimpleWithDisplays.Add(entity);

						var audit = new Audit();
						audit.CreatedBy = "Someone";
						context.SaveChanges(audit);

					}

					using (var context = new ModelAndContext.EntityContext())
					{
						var entityEFplus = context.AuditEntries.Include(x => x.Properties).Where(x => x.EntityTypeName ==  entity.GetType().GetAuditDisplayName()).First();

						Assert.AreEqual(entity.ColumInt, int.Parse(((string)entityEFplus.Properties.Where(x => x.PropertyName == "ColumInt").First().NewValue)));
					}

					using (var context = new ModelAndContext.EntityContext())
					{
						var entityEFplus = context.AuditEntries.Where<AuditEntry, EntitySimpleWithDisplay>(entity.Id).First();

						Assert.AreEqual(entity.ColumInt, int.Parse(((string)entityEFplus.Properties.Where(x => x.PropertyName == "ColumInt").First().NewValue)));
					}
				}
				finally
				{
					AuditManager.DefaultConfiguration.AutoSavePreAction = old;
				}

			}

			[TestMethod]
			public void Test_4()
			{
				Clean();
				var entity = new EntitySimpleWithDisplay();
				var oldConfig = AuditManager.DefaultConfiguration;
				var old = AuditManager.DefaultConfiguration.AutoSavePreAction;

				AuditManager.DefaultConfiguration = new AuditConfiguration();
				try
				{

					// some stuff can be modified depending on Mikael or Jonathan context 
					AuditManager.DefaultConfiguration.AutoSavePreAction = (context, audit) =>
						(context as ModelAndContext.EntityContext).AuditEntries.AddRange(audit.Entries);

					//AuditManager.DefaultConfiguration.DataAnnotationDisplayName();
					using (var context = new ModelAndContext.EntityContext())
					{
						entity.ColumInt = 125;

						context.EntitySimpleWithDisplays.Add(entity);

						var audit = new Audit();
						audit.CreatedBy = "Someone";
						context.SaveChanges(audit);

					}

					using (var context = new ModelAndContext.EntityContext())
					{
						var entityEFplus = context.AuditEntries.Include(x => x.Properties).Where(x => x.EntityTypeName == entity.GetType().Name).First();

						Assert.AreEqual(entity.ColumInt, int.Parse(((string)entityEFplus.Properties.Where(x => x.PropertyName == "ColumInt").First().NewValue)));
					}
				}
				finally
				{
					AuditManager.DefaultConfiguration = oldConfig;
					AuditManager.DefaultConfiguration.AutoSavePreAction = old;
				}
			}
		}
	}
}
