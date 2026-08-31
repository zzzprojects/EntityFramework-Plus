#if !INMEMORY && EFCORE_6X
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus.EFCore.Shared.MikaelAreaIndependant
{
	[TestClass]
	public class JoinSameNameError
	{
		private const string c_localDatabase = "Data Source=localhost;Initial Catalog=EFPLusSameNameComplexeCas;Persist Security Info=True;Trusted_Connection=Yes;MultipleActiveResultSets=False;TrustServerCertificate=True;";




		[TestMethod()]
		public void JoinSameNameError_1()
		{
			var optionsBuilder = new DbContextOptionsBuilder<TestContext>();
			optionsBuilder.UseSqlServer(c_localDatabase);

			using (var dbContext = new TestContext(optionsBuilder.Options))
			{
				try
				{

					dbContext.Database.EnsureDeleted();
				}
				catch
				(Exception ex)
				{
				}

				dbContext.Database.EnsureCreated();


				var storeIds = new[] { Guid.NewGuid() };

				var query1 = dbContext.Products.Where(x => storeIds.Contains(x.StoreId)).Select(x => x.ProductId);
				var query2 = dbContext.Employees.Where(x => storeIds.Contains(x.StoreId)).Select(x => x.EmployeeId);
				var query3 = query1.Union(query2);

				dbContext.Products.AsQueryable().Future();
				var t = query3.Future().ToList();

				Assert.AreEqual(0, t.Count());
			} 
		}


		public class Store
		{
			public Guid StoreId { get; set; }
			public List<Product> Products { get; set; } = new List<Product>();
			//public List<Employee> Employees { get; set; } = [];
		}

		public class Product
		{
			public Guid ProductId { get; }
			public Guid StoreId { get; set; }
		}

		public class Employee
		{
			public Guid EmployeeId { get; set; }
			public Guid StoreId { get; set; }
		}
		static void CreateSchemaAsync(DbContext dbContext)
		{
			var createStoresTable = @"
            IF OBJECT_ID(N'dbo.stores', N'U') IS NULL
            BEGIN
                CREATE TABLE stores (
                    store_id UNIQUEIDENTIFIER PRIMARY KEY
                )
            END";

			dbContext.Database.ExecuteSqlRaw(createStoresTable);

			var createProductsTable = @"
            IF OBJECT_ID(N'dbo.products', N'U') IS NULL
            BEGIN
                CREATE TABLE products (
                    product_id UNIQUEIDENTIFIER PRIMARY KEY,
                    store_id UNIQUEIDENTIFIER
                )
            END";

			dbContext.Database.ExecuteSqlRaw(createProductsTable);

			var createEmployeesTable = @"
            IF OBJECT_ID(N'dbo.employees', N'U') IS NULL
            BEGIN
                CREATE TABLE employees (
                    employee_id UNIQUEIDENTIFIER PRIMARY KEY,
                    store_id UNIQUEIDENTIFIER
                )
            END";

			dbContext.Database.ExecuteSqlRaw(createEmployeesTable);
		}

		public sealed class StoreConfiguration : IEntityTypeConfiguration<Store>
		{
			public void Configure(EntityTypeBuilder<Store> builder)
			{
				builder.ToTable("stores");
				builder.HasKey(x => x.StoreId);
				builder.Property(x => x.StoreId).HasColumnName("store_id").IsRequired();
			}
		}

		public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
		{
			public void Configure(EntityTypeBuilder<Product> builder)
			{
				builder.ToTable("products");
				builder.HasKey(x => x.ProductId);
				builder.Property(x => x.ProductId).HasColumnName("product_id").ValueGeneratedOnAdd();
				builder.Property(x => x.StoreId).HasColumnName("store_id").IsRequired();
			}
		}

		public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
		{
			public void Configure(EntityTypeBuilder<Employee> builder)
			{
				builder.ToTable("employees");
				builder.HasKey(x => x.EmployeeId);
				builder.Property(x => x.EmployeeId).HasColumnName("employee_id").ValueGeneratedOnAdd();
				builder.Property(x => x.StoreId).HasColumnName("store_id").IsRequired();
			}
		}

		public class TestContext : DbContext
		{
			public DbSet<Product> Products => Set<Product>();
			public DbSet<Employee> Employees => Set<Employee>();

			public TestContext(DbContextOptions primaryOptions) : base(primaryOptions)
			{
			}

			protected override void OnModelCreating(ModelBuilder modelBuilder)
			{
				modelBuilder.ApplyConfiguration(new StoreConfiguration());
				modelBuilder.ApplyConfiguration(new ProductConfiguration());
				modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
			}
		}


	}
}
#endif