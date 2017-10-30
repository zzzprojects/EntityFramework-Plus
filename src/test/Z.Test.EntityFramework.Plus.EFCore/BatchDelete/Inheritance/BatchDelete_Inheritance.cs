using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;
using Z.Test.EntityFramework.Plus.EFCore._Helper.Extensions;

namespace Z.Test.EntityFramework.Plus
{
	public partial class BatchDelete_Inheritance
	{
		[TestMethod]
		public void DeleteAllTPT()
		{
			//TPT is not supported in EFCore
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
