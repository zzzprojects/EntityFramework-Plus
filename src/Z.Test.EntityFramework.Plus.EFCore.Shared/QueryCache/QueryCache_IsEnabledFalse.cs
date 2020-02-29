using System;
using System.Linq; 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics; 
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using Z.EntityFramework.Plus; 

namespace Z.Test.EntityFramework.Plus.QueryCache
{
    [TestClass]
    public class QueryCache_IsEnabledFalse
    {
        public class FakeContext : DbContext
        {
            public DbSet<FakeModel> FakeModels { get; set; }

            public FakeContext(DbContextOptions<FakeContext> options) : base(options)
            {
            }
        }

        public class FakeModel
        {
            [Key]
            public Guid FakeId { get; set; }

            public string FakeString { get; set; }
        }
        [TestMethod]
        public  void QueryCache()
        {

            var optionsBuilder = new DbContextOptionsBuilder<FakeContext>();
            optionsBuilder.UseInMemoryDatabase("testName")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            var fakeContext = new FakeContext(optionsBuilder.Options);
             QueryCacheManager.UseFirstTagAsCacheKey = true;
   

            fakeContext.FakeModels.RemoveRange(fakeContext.FakeModels.ToList());
            fakeContext.SaveChanges();
             
            fakeContext.Add(new FakeModel { FakeId = Guid.NewGuid(), FakeString = "fakeString" });
            QueryCacheManager.IsEnabled = false;


            fakeContext.SaveChanges();
            try
            {
                
          
            var fakeModels = fakeContext
                .FakeModels
                .FromCache(typeof(FakeModel).Name)
                .ToList();


            var fakeModels2 = fakeContext
                .FakeModels
                .FromCache(typeof(FakeModel).Name)
                .ToList();
             


            Assert.AreEqual(1, fakeModels.Count);
            Assert.AreEqual(1, fakeModels2.Count);
            Assert.AreEqual(1, fakeContext
                .FakeModels
                .DeferredCount()
                .FromCache(typeof(FakeModel).Name));
            }
            finally
            {
                QueryCacheManager.UseFirstTagAsCacheKey = false;
                QueryCacheManager.IsEnabled = true;
            }
        }

        [TestMethod]
        public  void QueryCacheAsyn()
        {

            var optionsBuilder = new DbContextOptionsBuilder<FakeContext>();
            optionsBuilder.UseInMemoryDatabase("testName")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            var fakeContext = new FakeContext(optionsBuilder.Options);
            QueryCacheManager.UseFirstTagAsCacheKey = true;

            fakeContext.FakeModels.RemoveRange(fakeContext.FakeModels.ToList());
            fakeContext.SaveChanges();

            fakeContext.Add(new FakeModel { FakeId = Guid.NewGuid(), FakeString = "fakeString" });
            QueryCacheManager.IsEnabled = false;


            fakeContext.SaveChanges();

            try
            { 
            var fakeModels = fakeContext
                .FakeModels
                .FromCacheAsync(typeof(FakeModel).Name).Result
                .ToList();


            var fakeModels2 = fakeContext
                .FakeModels
                .FromCacheAsync(typeof(FakeModel).Name).Result
                .ToList();

            Assert.AreEqual(1, fakeModels.Count);
            Assert.AreEqual(1, fakeModels2.Count);
            Assert.AreEqual(1, fakeContext
                .FakeModels
                .DeferredCount()
                .FromCacheAsync(typeof(FakeModel).Name).Result);
            }
            finally
            {
                QueryCacheManager.UseFirstTagAsCacheKey = false;
                QueryCacheManager.IsEnabled = true;
            }
        }
    }
}
