using System;
using System.Collections.Generic;
using System.Text;
#if EFCORE_3X
#if !INMEMORY
using Microsoft.Data.SqlClient;
#endif
#else
using System.Data.SqlClient;
#endif
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus.EFCore.Shared.MikaelAreaIndependant
{
	public class ModelAndContext
    {
        public class My
        {
#if !EFCORE_3X
			private static string Database = "TestEFPlusCore20";
#elif EFCORE_3X
            private static string Database = "TestEFPlusCore30";
#endif

#if EFCORE_7X
			// [REPLACE] is in Beta.
			public static string ConnectionString = ("Server=localhost;Initial Catalog = [BD]; Integrated Security=True; Connection Timeout = 300; Persist Security Info=True;TrustServerCertificate=true").Replace("[REPLACE]", Environment.MachineName)
                .Replace("[BD]", Database);
#else
			// [REPLACE] is in Beta.
			public static string ConnectionString = ("Server=localhost;Initial Catalog = [BD]; Integrated Security=True; Connection Timeout = 300; Persist Security Info=True;").Replace("[REPLACE]", Environment.MachineName)
				.Replace("[BD]", Database);
#endif

			public static void DeleteBD(DbContext context)
            {
                context.Database.EnsureCreated();
                context.Database.EnsureDeleted();
            }
        }
		private static readonly ValueConverter<string, Guid> _guidConverter = new ValueConverter<string, Guid>(v => new Guid(v), v => v.ToString("N").ToUpperInvariant());

#if EFCORE_7X
		public static string ConnectionString =
			("Server=[REPLACE];Initial Catalog = [BD]; Integrated Security=True; Connection Timeout = 300; Persist Security Info=True;TrustServerCertificate=true").Replace("[REPLACE]", Environment.MachineName).Replace("[BD]", "EFPlusCore");
#else
		public static string ConnectionString =
			("Server=[REPLACE];Initial Catalog = [BD]; Integrated Security=True; Connection Timeout = 300; Persist Security Info=True;TrustServerCertificate=true").Replace("[REPLACE]", Environment.MachineName).Replace("[BD]", "EFPlusCore");
#endif 



		public static bool IsRetryStrat;
        public class EntityContext : DbContext
        {
            static EntityContext()
            {
                using (var context = new ModelAndContext.EntityContext())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                }


			}

			public EntityContext()
			{
			}  

			protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			{
#if INMEMORY
				optionsBuilder.UseInMemoryDatabase("MIKAEL");			
#else
				if (!IsRetryStrat)
				{

					optionsBuilder.UseSqlServer(new SqlConnection(ConnectionString));
				}
				else
				{


					optionsBuilder.UseSqlServer(new SqlConnection(ConnectionString)
						, option => option.EnableRetryOnFailure());
				}

#endif
				base.OnConfiguring(optionsBuilder);
			}
			public DbSet<AuditEntry> AuditEntries { get; set; }
			public DbSet<AuditEntryProperty> AuditEntryProperties { get; set; }

			public DbSet<EntitySimpleHasConverterSimple> EntitySimpleHasConverterSimples { get; set; }


			// includebyPath area BEGIN

			public DbSet<EntitySimpleForPath> EntitySimpleForPaths { get; set; }
			public DbSet<EntitySimpleForPathChild1> EntitySimpleForPathChild1s { get; set; }
			public DbSet<EntitySimpleForPathChild2> EntitySimpleForPathChild2s { get; set; }
			public DbSet<EntitySimpleForPathChild3> EntitySimpleForPathChild3s { get; set; }
			public DbSet<EntitySimpleForPathChildList1> EntitySimpleForPathChildList1s { get; set; }
			public DbSet<EntitySimpleForPathChildList2> EntitySimpleForPathChildList2s { get; set; }
			public DbSet<EntitySimpleForPathChildList3> EntitySimpleForPathChildList3s { get; set; } 

			// includebyPath area END

			public DbSet<EntitySimpleCacheOnlyDontNewUse> EntitySimpleCacheOnlyDontNewUses { get; set; }

            public DbSet<Blog> Blogs { get; set; }
			public DbSet<Post> Posts { get; set; }
			public DbSet<Comment> Comments { get; set; }
			public DbSet<EntitySimple> EntitySimples { get; set; }
			public DbSet<EntitySimpleChild> EntitySimpleChilds { get; set; }

			// AREA IncludeOptimized inheritance  BEGIN
			public virtual DbSet<Entity> Entities { get; set; }
			// AREA IncludeOptimized inheritance  END

			public DbSet<EntitySimpleWithDisplay> EntitySimpleWithDisplays { get; set; }

			protected override void OnModelCreating(ModelBuilder modelBuilder)
			{

				modelBuilder.Entity<EntitySimpleHasConverterSimple>().Property(e => e.ColumnGuid).HasColumnName("value").HasColumnType("UNIQUEIDENTIFIER").HasConversion(_guidConverter);
				// AREA IncludeOptimized inheritance  BEGIN
				modelBuilder.Entity<Entity>(entity =>
				{
					entity.ToTable("Container");
					entity.HasKey(e => e.Id);
					entity.HasDiscriminator<EntityTypes>("Type")
						.HasValue<Entity>(EntityTypes.Unknown)
						.HasValue<Derived>(EntityTypes.Type1)
						.HasValue<Derived2>(EntityTypes.Type2);
				});

				modelBuilder.Entity<Derived>(entity =>
				{
					entity.HasBaseType<Entity>();
					entity.HasMany(e => e.Includes).WithOne().HasForeignKey("Device_FK").OnDelete(DeleteBehavior.Cascade);
				});

				modelBuilder.Entity<Derived2>(entity =>
				{
					entity.HasBaseType<Entity>();
				});

				modelBuilder.Entity<DerivedInclude>(entity =>
				{
					entity.ToTable("Module");
					entity.HasKey(e => e.Id);
					entity.Property(e => e.SomeProp).HasColumnName("Model");
				});

				// AREA IncludeOptimized inheritance  END
			}
		}

        public class EntitySimpleCacheOnlyDontNewUse
        {
            public int Id { get; set; }
            public int ColumnInt { get; set; }
            public string ColumnString { get; set; }
        }
    }
	// AREA IncludeOptimized inheritance  BEGIN
	// Model
	public enum EntityTypes
	{
		Unknown = 0,
		Type1,
		Type2
	}

	public class Entity
	{
		public int Id { get; set; }
		public EntityTypes Type { get; set; }
	}

	public class Derived : Entity
	{
		public List<DerivedInclude> Includes { get; set; }
	}

	public class Derived2 : Entity
	{
	}

	public class DerivedInclude
	{
		public int Id { get; set; }
		public string SomeProp { get; set; }
	}

	// AREA IncludeOptimized inheritance  END

	// Zone HasConverted
	public class EntitySimpleHasConverterSimple
	{
		public int ID { get; set; } 
		public string ColumnGuid { get; set; } 
	} 
	 
	public class Blog
	{
		public int BlogId { get; set; }
		public string Title { get; set; }
		public bool IsSoftDeleted { get; set; }
		public List<Post> Posts { get; set; }
	}

	public class Post
	{
		public int PostId { get; set; }
		public string Content { get; set; }
		public DateTime Date { get; set; }
		public bool IsSoftDeleted { get; set; }
		public int Price { get; set; }
		public int BlogId { get; set; }
		public List<Comment> Comments { get; set; }
	}

	public class Comment
	{
		public int CommentId { get; set; }
		public string Text { get; set; }
		public bool IsSoftDeleted { get; set; }
		public int PostId { get; set; }
	}

	public class EntitySimple
	{
		public int ID { get; set; }
		public int ColumnInt { get; set; }
		public int? ColumnIntNullable { get; set; }
		public string ColumnString { get; set; }
		public List<EntitySimpleChild> EntitySimpleChild { get; set; }
	}

	public class EntitySimpleChild
	{
		public int ID { get; set; }
		public int ColumnInt { get; set; }
		public string ColumnString { get; set; }
	}

	// includebyPath area BEGIN

	public class EntitySimpleForPath
	{
		public int ID { get; set; }
		public int ColumnInt { get; set; }
		public string ColumnString { get; set; } 
		public EntitySimpleForPathChild1 EntitySimpleForPathChild1 { get; set; }
		public EntitySimpleForPathChild2 EntitySimpleForPathChild2 { get; set; }
		public EntitySimpleForPathChild3 EntitySimpleForPathChild3 { get; set; }
		public List<EntitySimpleForPathChildList1> EntitySimpleForPathChildList1s { get; set; }
		public List<EntitySimpleForPathChildList2> EntitySimpleForPathChildList2s { get; set; }
		public List<EntitySimpleForPathChildList3> EntitySimpleForPathChildList3s { get; set; }
	}

	public class EntitySimpleForPathChild1
	{
		public int ID { get; set; }
		public int ColumnInt { get; set; }
		public string ColumnString { get; set; }
	}

	public class EntitySimpleForPathChild2
	{
		public int ID { get; set; }
		public int ColumnInt { get; set; }
		public string ColumnString { get; set; }
	}

	public class EntitySimpleForPathChild3
	{
		public int ID { get; set; }
		public int ColumnInt { get; set; }
		public string ColumnString { get; set; }
	}

	public class EntitySimpleForPathChildList1
	{
		public int ID { get; set; }
		public int ColumnInt { get; set; }
		public string ColumnString { get; set; }
	}

	public class EntitySimpleForPathChildList2
	{
		public int ID { get; set; }
		public int ColumnInt { get; set; }
		public string ColumnString { get; set; }
	}

	public class EntitySimpleForPathChildList3
	{
		public int ID { get; set; }
		public int ColumnInt { get; set; }
		public string ColumnString { get; set; }
	}
	// includebyPath area END


	[AuditDisplay("EntitySimple_Patate")]
	public class EntitySimpleWithDisplay
	{
		public int Id { get; set; }
		public int ColumInt { get; set; }
		public string ColumString { get; set; }
	}
}
