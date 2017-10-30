using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Z.Test.EntityFramework.Plus
{
	public static class TestContextExtensions
	{
		public static List<T> Insert<T>(this TestContext dcContext, int intCount) where T: class, new()
		{
			DbSet<T> dbSet = dcContext.Set<T>();

			if (dbSet == null)
			{
				return null;
			}

			int countBefore = dbSet.Count();

			List<T> list = new List<T>();
			for (int i = 0; i < intCount; i++)
			{
				T item = new T();
				TestContext.InsertFactory(item, i);
				list.Add(item);
			}

			dbSet.AddRange(list);
			dcContext.SaveChanges();

			Assert.AreEqual(intCount + countBefore, dbSet.Count());

			return list;
		}

		public static void DeleteAll<T>(this TestContext dcContext) where T : class
		{
			DbSet<T> dbSet = dcContext.Set<T>();

			dbSet.RemoveRange(dbSet);
			dcContext.SaveChanges();

			Assert.AreEqual(0, dbSet.Count());
		}
	}
}
