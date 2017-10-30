using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
	public partial class BatchDelete_Inheritance
	{
		[TestMethod]
		public void DeleteAllTPT()
		{
			using (TestContext tcContext = new TestContext())
			{
				//make sure our FK between Animals and Dogs supports cascade
				tcContext.Database.ExecuteSqlCommand(@"
					ALTER TABLE [dbo].[Inheritance_TPT_Dogs] DROP CONSTRAINT [FK_dbo.Inheritance_TPT_Dogs_dbo.Inheritance_TPT_Animals_ID];

					ALTER TABLE [dbo].[Inheritance_TPT_Dogs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Inheritance_TPT_Dogs_dbo.Inheritance_TPT_Animals_ID] FOREIGN KEY([ID])
					REFERENCES [dbo].[Inheritance_TPT_Animals] ([ID])
					ON UPDATE CASCADE
					ON DELETE CASCADE;

					ALTER TABLE [dbo].[Inheritance_TPT_Dogs] CHECK CONSTRAINT [FK_dbo.Inheritance_TPT_Dogs_dbo.Inheritance_TPT_Animals_ID];

					ALTER TABLE [dbo].[Inheritance_TPT_Cats] DROP CONSTRAINT [FK_dbo.Inheritance_TPT_Cats_dbo.Inheritance_TPT_Animals_ID];

					ALTER TABLE [dbo].[Inheritance_TPT_Cats]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Inheritance_TPT_Cats_dbo.Inheritance_TPT_Animals_ID] FOREIGN KEY([ID])
					REFERENCES [dbo].[Inheritance_TPT_Animals] ([ID])
					ON UPDATE CASCADE
					ON DELETE CASCADE;

					ALTER TABLE [dbo].[Inheritance_TPT_Cats] CHECK CONSTRAINT [FK_dbo.Inheritance_TPT_Cats_dbo.Inheritance_TPT_Animals_ID];
				");

				//clear all animals
				tcContext.DeleteAll<Inheritance_TPT_Animal>();

				//add 50 new dogs
				tcContext.Insert<Inheritance_TPT_Dog>(50);

				//add 25 cats
				tcContext.Insert<Inheritance_TPT_Cat>(25);

				//add some diverity to our data
				int intRowsAffected = tcContext
					.Inheritance_TPT_Cats
					.Take(10)
					.Update(i => new Inheritance_TPT_Cat()
					{
						ColumnCat = 888,
						ColumnInt = 2
					});

				//we should have 20 affected rows
				Assert.AreEqual(20, intRowsAffected);

				//add some diverity to our data
				intRowsAffected = tcContext
					.Inheritance_TPT_Dogs
					.Take(10)
					.Update(i => new Inheritance_TPT_Dog()
					{
						ColumnDog = 999,
						ColumnInt = 1
					});

				//we should have 20 affected rows
				Assert.AreEqual(20, intRowsAffected);

				//delete our dogs
				intRowsAffected = tcContext.Inheritance_TPT_Dogs.Where(i => i.ColumnDog == 999).Delete();

				//we should have 10 affected rows
				Assert.AreEqual(10, intRowsAffected);

				//verify that they were deleted properly, we should have 40 dogs remaining
				Assert.AreEqual(0, tcContext.Inheritance_TPT_Dogs.Count(i => i.ColumnDog == 999));
				Assert.AreEqual(40, tcContext.Inheritance_TPT_Dogs.Count());

				//delete our cats
				intRowsAffected = tcContext.Inheritance_TPT_Cats.Where(i => i.ColumnCat == 888).Delete();

				//we should have 10 affected rows
				Assert.AreEqual(10, intRowsAffected);

				//verify that they were deleted properly, we should have 15 cats remaining
				Assert.AreEqual(0, tcContext.Inheritance_TPT_Cats.Count(i => i.ColumnCat == 888));
				Assert.AreEqual(15, tcContext.Inheritance_TPT_Cats.Count());

				//we should have 55 animals in total
				Assert.AreEqual(55, tcContext.Inheritance_TPT_Animals.Count());

				//undo our change
				tcContext.Database.ExecuteSqlCommand(@"
					ALTER TABLE [dbo].[Inheritance_TPT_Dogs] DROP CONSTRAINT [FK_dbo.Inheritance_TPT_Dogs_dbo.Inheritance_TPT_Animals_ID];

					ALTER TABLE [dbo].[Inheritance_TPT_Dogs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Inheritance_TPT_Dogs_dbo.Inheritance_TPT_Animals_ID] FOREIGN KEY([ID])
					REFERENCES [dbo].[Inheritance_TPT_Animals] ([ID]);

					ALTER TABLE [dbo].[Inheritance_TPT_Dogs] CHECK CONSTRAINT [FK_dbo.Inheritance_TPT_Dogs_dbo.Inheritance_TPT_Animals_ID];

					ALTER TABLE [dbo].[Inheritance_TPT_Cats] DROP CONSTRAINT [FK_dbo.Inheritance_TPT_Cats_dbo.Inheritance_TPT_Animals_ID];

					ALTER TABLE [dbo].[Inheritance_TPT_Cats]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Inheritance_TPT_Cats_dbo.Inheritance_TPT_Animals_ID] FOREIGN KEY([ID])
					REFERENCES [dbo].[Inheritance_TPT_Animals] ([ID]);

					ALTER TABLE [dbo].[Inheritance_TPT_Cats] CHECK CONSTRAINT [FK_dbo.Inheritance_TPT_Cats_dbo.Inheritance_TPT_Animals_ID];
				");
			}
		}

		[TestMethod]
		public void DeleteAllTPH()
		{
			using (TestContext tcContext = new TestContext())
			{
				//clear all animals
				tcContext.DeleteAll<Inheritance_TPH_Animal>();

				//add 50 new dogs
				tcContext.Insert<Inheritance_TPH_Dog>(50);

				//add 25 cats
				tcContext.Insert<Inheritance_TPH_Cat>(25);

				//add some diverity to our data
				int intRowsAffected = tcContext
					.Inheritance_TPH_Animals
					.OfType<Inheritance_TPH_Cat>()
					.Take(10)
					.Update(i => new Inheritance_TPH_Cat()
					{
						ColumnCat = 888,
						ColumnInt = 2
					});

				//we should have 10 affected rows
				Assert.AreEqual(10, intRowsAffected);

				//add some diverity to our data
				intRowsAffected = tcContext
					.Inheritance_TPH_Animals
					.OfType<Inheritance_TPH_Dog>()
					.Take(10)
					.Update(i => new Inheritance_TPH_Dog()
					{
						ColumnDog = 999,
						ColumnInt = 1
					});

				//we should have 10 affected rows
				Assert.AreEqual(10, intRowsAffected);

				//delete our dogs
				intRowsAffected = tcContext.Inheritance_TPH_Animals.OfType<Inheritance_TPH_Dog>().Where(i => i.ColumnDog == 999).Delete();

				//we should have 10 affected rows
				Assert.AreEqual(10, intRowsAffected);

				//verify that they were deleted properly, we should have 40 dogs remaining
				Assert.AreEqual(0, tcContext.Inheritance_TPH_Animals.OfType<Inheritance_TPH_Dog>().Count(i => i.ColumnDog == 999));
				Assert.AreEqual(40, tcContext.Inheritance_TPH_Animals.OfType<Inheritance_TPH_Dog>().Count());

				//delete our cats
				intRowsAffected = tcContext.Inheritance_TPH_Animals.OfType<Inheritance_TPH_Cat>().Where(i => i.ColumnCat == 888).Delete();

				//we should have 10 affected rows
				Assert.AreEqual(10, intRowsAffected);

				//verify that they were deleted properly, we should have 15 cats remaining
				Assert.AreEqual(0, tcContext.Inheritance_TPH_Animals.OfType<Inheritance_TPH_Cat>().Count(i => i.ColumnCat == 888));
				Assert.AreEqual(15, tcContext.Inheritance_TPH_Animals.OfType<Inheritance_TPH_Cat>().Count());

				//we should have 55 animals in total
				Assert.AreEqual(55, tcContext.Inheritance_TPH_Animals.Count());
			}
		}

		[TestMethod]
		public void DeleteAllTPC()
		{
			using (TestContext tcContext = new TestContext())
			{
				//clear all animals
				tcContext.DeleteAll<Inheritance_TPC_Cat>();
				tcContext.DeleteAll<Inheritance_TPC_Dog>();

				//add 50 new dogs
				tcContext.Insert<Inheritance_TPC_Dog>(50);

				//add 25 cats
				tcContext.Insert<Inheritance_TPC_Cat>(25);

				//add some diverity to our data
				int intRowsAffected = tcContext
					.Inheritance_TPC_Cats
					.Take(10)
					.Update(i => new Inheritance_TPC_Cat()
					{
						ColumnCat = 888,
						ColumnInt = 2
					});

				//we should have 10 affected rows
				Assert.AreEqual(10, intRowsAffected);

				//add some diverity to our data
				intRowsAffected = tcContext
					.Inheritance_TPC_Dogs
					.Take(10)
					.Update(i => new Inheritance_TPC_Dog()
					{
						ColumnDog = 999,
						ColumnInt = 1
					});

				//we should have 10 affected rows
				Assert.AreEqual(10, intRowsAffected);

				//delete our dogs
				intRowsAffected = tcContext.Inheritance_TPC_Dogs.Where(i => i.ColumnDog == 999).Delete();

				//we should have 10 affected rows
				Assert.AreEqual(10, intRowsAffected);

				//verify that they were deleted properly, we should have 40 dogs remaining
				Assert.AreEqual(0, tcContext.Inheritance_TPC_Dogs.Count(i => i.ColumnDog == 999));
				Assert.AreEqual(40, tcContext.Inheritance_TPC_Dogs.Count());

				//delete our cats
				intRowsAffected = tcContext.Inheritance_TPC_Cats.Where(i => i.ColumnCat == 888).Delete();

				//we should have 10 affected rows
				Assert.AreEqual(10, intRowsAffected);

				//verify that they were deleted properly, we should have 15 cats remaining
				Assert.AreEqual(0, tcContext.Inheritance_TPC_Cats.Count(i => i.ColumnCat == 888));
				Assert.AreEqual(15, tcContext.Inheritance_TPC_Cats.Count());
			}
		}
	}
}
