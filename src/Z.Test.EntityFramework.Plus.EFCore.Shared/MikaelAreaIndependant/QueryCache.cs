#if !INMEMORY
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus.EFCore.Shared.MikaelAreaIndependant
{
	[TestClass]
	public class QueryCache
	{

		[TestMethod()]
		public void QueryCache_1()
		{
			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimples.RemoveRange(context.EntitySimples);
				context.EntitySimpleChilds.RemoveRange(context.EntitySimpleChilds);

				context.SaveChanges();
			}


			// SEED  
			using (var context = new ModelAndContext.EntityContext())
			{
				for (int i = 0; i < 3; i++)
				{
					context.EntitySimples.Add(new EntitySimple { ColumnInt = i, ColumnString = "test" });
				}

				context.SaveChanges();
			}

			// TEST  
			using (var context = new ModelAndContext.EntityContext())
			{
				var c = context.EntitySimples.AsNoTracking().Where(x => x.ColumnString == "test").FromCache().ToList();
				var lc = context.EntitySimples.AsNoTracking().Where(x => x.ColumnString == "test").FromCache("teast").ToList();
				var llc = context.EntitySimples.AsNoTracking().Where(x => x.ColumnString == "test").FromCache().ToList();

				Assert.AreEqual(3, lc.Count);
				Assert.AreEqual(3, c.Count);
				Assert.AreEqual(3, llc.Count);
			}

			// TEST  
			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimples.Where(x => x.ColumnString == "test").Take(1).Delete();
				var c = context.EntitySimples.AsNoTracking().Where(x => x.ColumnString == "test").FromCache().ToList();


				QueryCacheManager.ExpireAll();

				var bc = context.EntitySimples.AsNoTracking().Where(x => x.ColumnString == "test").FromCache().ToList();
				var jjc = context.EntitySimples.AsNoTracking().Where(x => x.ColumnString == "test").FromCache("teast").ToList();
				var jjdc = context.EntitySimples.AsNoTracking().Where(x => x.ColumnString == "test").FromCache("test").ToList();

				Assert.AreEqual(2, bc.Count);
				Assert.AreEqual(2, jjc.Count);
				Assert.AreEqual(2, jjdc.Count);
				Assert.AreEqual(3, c.Count);
			}

			// TEST  
			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimples.Where(x => x.ColumnString == "test").Take(1).Delete();
				var c = context.EntitySimples.AsNoTracking().Where(x => x.ColumnString == "test").FromCache("test").ToList();

				var dc = context.EntitySimples.AsNoTracking().Where(x => x.ColumnString == "test").FromCache().ToList();

				QueryCacheManager.ExpireType(typeof(EntitySimple));

				var bc = context.EntitySimples.AsNoTracking().Where(x => x.ColumnString == "test").FromCache().ToList();
				var bac = context.EntitySimples.AsNoTracking().Where(x => x.ColumnString == "test").FromCache("test").ToList();

				Assert.AreEqual(1, bc.Count);
				Assert.AreEqual(1, bac.Count);
				Assert.AreEqual(2, c.Count);
				Assert.AreEqual(2, dc.Count);
			}

			// TEST  
			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimples.Where(x => x.ColumnString == "test").Take(1).Delete();
				var c = context.EntitySimples.AsNoTracking().Where(x => x.ColumnString == "test").FromCache("test").ToList();

				var dc = context.EntitySimples.AsNoTracking().Where(x => x.ColumnString == "test").FromCache().ToList();

				context.EntitySimples.ExpireCache();

				var bc = context.EntitySimples.AsNoTracking().Where(x => x.ColumnString == "test").FromCache().ToList();
				var bac = context.EntitySimples.AsNoTracking().Where(x => x.ColumnString == "test").FromCache("test").ToList();

				Assert.AreEqual(0, bc.Count);
				Assert.AreEqual(0, bac.Count);
				Assert.AreEqual(1, c.Count);
				Assert.AreEqual(1, dc.Count);
			}
		}

#if EFCORE_5X

		[TestMethod()]
		public void QueryCache_2()
		{
			using (var context = new ModelAndContext.EntityContext())
			{
				context.RemoveRange(context.EntitySimpleForPaths);
				context.RemoveRange(context.EntitySimpleForPathChild1s);
				context.RemoveRange(context.EntitySimpleForPathChild2s);
				context.RemoveRange(context.EntitySimpleForPathChild3s);
				context.RemoveRange(context.EntitySimpleForPathChildList1s);
				context.RemoveRange(context.EntitySimpleForPathChildList2s);
				context.RemoveRange(context.EntitySimpleForPathChildList3s);
				context.SaveChanges();
			}


			// SEED  
			using (var context = new ModelAndContext.EntityContext())
			{
				List<EntitySimpleForPath> list = new List<EntitySimpleForPath>();
				for (int i = 0; i < 2; i++)
				{
					var child = new EntitySimpleForPathChild1() { ColumnInt = i };
					list.Add(new EntitySimpleForPath { ColumnInt = i, ColumnString = "test", EntitySimpleForPathChild1 = child });
					list.Add(new EntitySimpleForPath { ColumnInt = i, ColumnString = "test", EntitySimpleForPathChild1 = child });
				}

				context.BulkInsert(list, option => option.IncludeGraph = true);
			}

			// TEST  
			using (var context = new ModelAndContext.EntityContext())
			{
				List<EntitySimpleForPath> list1 = context.EntitySimpleForPaths.Include(x => x.EntitySimpleForPathChild1).Where(x => x.ColumnString == "test").FromCache("test").ToList();
				List<EntitySimpleForPath> list2 = new List<EntitySimpleForPath>();

				try
				{
					QueryCacheManager.UseAsNoTrackingWithIdentityResolution = true;

					list2 = context.EntitySimpleForPaths.Include(x => x.EntitySimpleForPathChild1).Where(x => x.ColumnString == "test").FromCache().ToList();

				}
				finally
				{

					QueryCacheManager.UseAsNoTrackingWithIdentityResolution = false;
				}

				Assert.AreEqual(4, list1.Select(x => x.EntitySimpleForPathChild1).Distinct().Count());
				Assert.AreEqual(2, list2.Select(x => x.EntitySimpleForPathChild1).Distinct().Count());
			}

		}
		[TestMethod()]
		public void QueryCache_3()
		{
			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimples.RemoveRange(context.EntitySimples);
				context.EntitySimpleChilds.RemoveRange(context.EntitySimpleChilds);

				context.SaveChanges();
			}


			// SEED  
			using (var context = new ModelAndContext.EntityContext())
			{
				for (int i = 0; i < 3; i++)
				{
					context.EntitySimples.Add(new EntitySimple { ColumnInt = i, ColumnString = "test" });
				}

				context.SaveChanges();
			}

			var list = new List<string>() { "test", "patate" };
			// TEST  
			using (var context = new ModelAndContext.EntityContext())
			{

				var c = context.EntitySimples.AsNoTracking().Where(x => list.Contains(x.ColumnString)).FromCache().ToList();
				var lc = context.EntitySimples.AsNoTracking().Where(x => list.Contains(x.ColumnString)).FromCache("teast").ToList();
				list.Clear();
				var llc = context.EntitySimples.AsNoTracking().Where(x => list.Contains(x.ColumnString)).FromCache().ToList();

				Assert.AreEqual(3, lc.Count);
				Assert.AreEqual(3, c.Count);
				Assert.AreEqual(0, llc.Count);
			}

			list = new List<string>() { "test", "patate" };

			// TEST  
			using (var context = new ModelAndContext.EntityContext())
			{

				var c = context.EntitySimples.AsNoTracking().Where(x => list.Contains(x.ColumnString)).FromCache().ToList();
				var lc = context.EntitySimples.AsNoTracking().Where(x => list.Contains(x.ColumnString)).FromCache("teast").ToList();
				var llc = context.EntitySimples.AsNoTracking().Where(x => list.Contains(x.ColumnString)).FromCache().ToList();

				Assert.AreEqual(3, lc.Count);
				Assert.AreEqual(3, c.Count);
				Assert.AreEqual(3, llc.Count);
			}
			// TEST  
			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimples.Where(x => list.Contains(x.ColumnString)).Take(1).Delete();
				var c = context.EntitySimples.AsNoTracking().Where(x => list.Contains(x.ColumnString)).FromCache().ToList();


				QueryCacheManager.ExpireAll();

				var bc = context.EntitySimples.AsNoTracking().Where(x => list.Contains(x.ColumnString)).FromCache().ToList();
				var jjc = context.EntitySimples.AsNoTracking().Where(x => list.Contains(x.ColumnString)).FromCache("teast").ToList();
				var jjdc = context.EntitySimples.AsNoTracking().Where(x => list.Contains(x.ColumnString)).FromCache("test").ToList();

				Assert.AreEqual(2, bc.Count);
				Assert.AreEqual(2, jjc.Count);
				Assert.AreEqual(2, jjdc.Count);
				Assert.AreEqual(3, c.Count);
			}

			// TEST  
			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimples.Where(x => list.Contains(x.ColumnString)).Take(1).Delete();
				var c = context.EntitySimples.AsNoTracking().Where(x => list.Contains(x.ColumnString)).FromCache("test").ToList();

				var dc = context.EntitySimples.AsNoTracking().Where(x => list.Contains(x.ColumnString)).FromCache().ToList();

				QueryCacheManager.ExpireType(typeof(EntitySimple));

				var bc = context.EntitySimples.AsNoTracking().Where(x => list.Contains(x.ColumnString)).FromCache().ToList();
				var bac = context.EntitySimples.AsNoTracking().Where(x => list.Contains(x.ColumnString)).FromCache("test").ToList();

				Assert.AreEqual(1, bc.Count);
				Assert.AreEqual(1, bac.Count);
				Assert.AreEqual(2, c.Count);
				Assert.AreEqual(2, dc.Count);
			}

			// TEST  
			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimples.Where(x => list.Contains(x.ColumnString)).Take(1).Delete();
				var c = context.EntitySimples.AsNoTracking().Where(x => list.Contains(x.ColumnString)).FromCache("test").ToList();

				var dc = context.EntitySimples.AsNoTracking().Where(x => list.Contains(x.ColumnString)).FromCache().ToList();

				context.EntitySimples.ExpireCache();

				var bc = context.EntitySimples.AsNoTracking().Where(x => list.Contains(x.ColumnString)).FromCache().ToList();
				var bac = context.EntitySimples.AsNoTracking().Where(x => list.Contains(x.ColumnString)).FromCache("test").ToList();

				Assert.AreEqual(0, bc.Count);
				Assert.AreEqual(0, bac.Count);
				Assert.AreEqual(1, c.Count);
				Assert.AreEqual(1, dc.Count);
			}
		}

        [TestMethod()]
        public void QueryCache_4()
        {

            ProductFilterModel filter1 = new ProductFilterModel()
            {
                Attributes = new List<FilterAttributeWrapper>()
                {
                    new FilterAttributeWrapper()
                    {
                        AttributeValues = new List<AttributeValueWrapper>()
                        {
                            new AttributeValueWrapper()
                            {
                                Url = "bookzone1"
                            }
                        }
                    }
                }
            };
            var filterValues1 = filter1.Attributes.Where(e => e.AttributeValues.Count() > 0).Select(e => new { AttributeValues = e.AttributeValues.Select(f => f.Url) });




            ProductFilterModel filter2 = new ProductFilterModel()
            {
                Attributes = new List<FilterAttributeWrapper>()
                {
                    new FilterAttributeWrapper()
                    {
                        AttributeValues = new List<AttributeValueWrapper>()
                        {
                            new AttributeValueWrapper()
                            {
                                Url = "bookzone2"
                            }
                        }
                    }
                }
            };
            var filterValues2 = filter2.Attributes.Where(e => e.AttributeValues.Count() > 0).Select(e => new { AttributeValues = e.AttributeValues.Select(f => f.Url) });
            ProductFilterModel filter3 = new ProductFilterModel()
            {
                Attributes = new List<FilterAttributeWrapper>()
                {
                    new FilterAttributeWrapper()
                    {
                        AttributeValues = new List<AttributeValueWrapper>()
                        {
                            new AttributeValueWrapper()
                            {
                                Url = "bookzone1"
                            }
                        }
                    }
                }
            };
            var filterValues3 = filter3.Attributes.Where(e => e.AttributeValues.Count() > 0).Select(e => new { AttributeValues = e.AttributeValues.Select(f => f.Url) });


            using (var context = new ModelAndContext.EntityContext())
            {
				var key1 = QueryCacheManager.GetCacheKey(context.EntitySimples.AsNoTracking().Where(x => filterValues1.FirstOrDefault().AttributeValues.Contains(x.ColumnString)), new string[] { });
                filterValues1 = filterValues2;
                var key2 = QueryCacheManager.GetCacheKey(context.EntitySimples.AsNoTracking().Where(x => filterValues1.FirstOrDefault().AttributeValues.Contains(x.ColumnString)), new string[] { });
				filterValues1 = filterValues3;
                var key3 = QueryCacheManager.GetCacheKey(context.EntitySimples.AsNoTracking().Where(x => filterValues1.FirstOrDefault().AttributeValues.Contains(x.ColumnString)), new string[] { });

                Assert.AreNotEqual(key1, key2);
                Assert.AreEqual(key1, key3);
            }

        }
        public class FilterAttributeWrapper
        {
            public IEnumerable<AttributeValueWrapper> AttributeValues { get; set; }
        }

        public class ProductFilterModel
        {
            public IEnumerable<FilterAttributeWrapper> Attributes { get; set; }
        }

        public class AttributeValueWrapper
        {
            public string Url { get; set; }
        }

#endif
    }
}
#endif