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
	public partial class BatchUpdate_Inheritance
	{
		[TestMethod]
		public void UpdateAllTPT()
		{
			//TPT is not supported in EFCore
		}

		[TestMethod]
		public void UpdateAllTPH()
		{
			using (TestContext tcContext = new TestContext())
			{
				//delete all the animals
				tcContext.DeleteAll<Inheritance_TPH_Animal>();

				//add 50 dogs
				tcContext.Insert<Inheritance_TPH_Dog>(50);

				//add 25 cats
				tcContext.Insert<Inheritance_TPH_Cat>(25);

				//update our dogs and the base animals
				int intRowsAffected = tcContext
					.Inheritance_TPH_Animals
					.OfType<Inheritance_TPH_Dog>()
					.Update(i => new Inheritance_TPH_Dog()
					{
						ColumnDog = 2,
						ColumnInt = 1
					});

				//we should have 50 affected rows.
				Assert.AreEqual(50, intRowsAffected);

				//update our dogs and the base animals
				intRowsAffected = tcContext
					.Inheritance_TPH_Animals
					.OfType<Inheritance_TPH_Cat>()
					.Update(i => new Inheritance_TPH_Cat()
					{
						ColumnCat = 3,
						ColumnInt = 2
					});

				//we should have 25 affected rows.
				Assert.AreEqual(25, intRowsAffected);

				//verify that the dogs were updated properly
				Assert.AreEqual(100, tcContext.Inheritance_TPH_Animals.OfType<Inheritance_TPH_Dog>().Sum(i => i.ColumnDog));
				Assert.AreEqual(50, tcContext.Inheritance_TPH_Animals.OfType<Inheritance_TPH_Dog>().Sum(i => i.ColumnInt));

				//verify that the cats were updated properly
				Assert.AreEqual(75, tcContext.Inheritance_TPH_Animals.OfType<Inheritance_TPH_Cat>().Sum(i => i.ColumnCat));
				Assert.AreEqual(50, tcContext.Inheritance_TPH_Animals.OfType<Inheritance_TPH_Cat>().Sum(i => i.ColumnInt));

				//there should 25 cats
				Assert.AreEqual(25, tcContext.Inheritance_TPH_Animals.OfType<Inheritance_TPH_Cat>().Count());

				//there should be 50 dogs
				Assert.AreEqual(50, tcContext.Inheritance_TPH_Animals.OfType<Inheritance_TPH_Dog>().Count());

				//there should be 75 animals
				Assert.AreEqual(75, tcContext.Inheritance_TPH_Animals.Count());
			}
		}

		[TestMethod]
		public void UpdateAllTPC()
		{
			using (TestContext tcContext = new TestContext())
			{
				//delete all the animals
				tcContext.DeleteAll<Inheritance_TPC_Cat>();
				tcContext.DeleteAll<Inheritance_TPC_Dog>();

				//add 50 dogs
				tcContext.Insert<Inheritance_TPC_Dog>(50);

				//add 25 cats
				tcContext.Insert<Inheritance_TPC_Cat>(25);

				//update our dogs and the base animals
				int intRowsAffected = tcContext.Inheritance_TPC_Dogs.Update(i => new Inheritance_TPC_Dog()
				{
					ColumnDog = 2,
					ColumnInt = 1
				});

				//we should have 50 affected rows as there's only one table and 50 dogs to update
				Assert.AreEqual(50, intRowsAffected);

				//verify that they were updated properly
				Assert.AreEqual(100, tcContext.Inheritance_TPC_Dogs.Sum(i => i.ColumnDog));
				Assert.AreEqual(50, tcContext.Inheritance_TPC_Dogs.Sum(i => i.ColumnInt));

				//update our dogs and the base animals
				intRowsAffected = tcContext.Inheritance_TPC_Cats.Update(i => new Inheritance_TPC_Cat()
				{
					ColumnCat = 3,
					ColumnInt = 2
				});

				//we should have 25 affected rows as there's only one table and 25 cats to update
				Assert.AreEqual(25, intRowsAffected);

				//verify that they were updated properly
				Assert.AreEqual(75, tcContext.Inheritance_TPC_Cats.Sum(i => i.ColumnCat));
				Assert.AreEqual(50, tcContext.Inheritance_TPC_Cats.Sum(i => i.ColumnInt));
			}
		}
	}
}
