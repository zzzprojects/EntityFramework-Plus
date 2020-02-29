using System;
using System.Collections.Generic;
using System.Text;
#if EFCORE_3X
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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


            // [REPLACE] is in Beta.
            public static string ConnectionString = ("Server=localhost;Initial Catalog = [BD]; User ID=test;password=test; Connection Timeout = 300; Persist Security Info=True;").Replace("[REPLACE]", Environment.MachineName)
                .Replace("[BD]", Database);
            public static void DeleteBD(DbContext context)
            {
                context.Database.EnsureCreated();
                context.Database.EnsureDeleted();
            }
        }


        public static string ConnectionString =
			("Server=[REPLACE];Initial Catalog = [BD]; User ID=test;password=test; Connection Timeout = 300; Persist Security Info=True").Replace("[REPLACE]", Environment.MachineName).Replace("[BD]", "EFPlusCore");


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
				optionsBuilder.UseSqlServer(new SqlConnection(ConnectionString));

				base.OnConfiguring(optionsBuilder);
			}

            public DbSet<EntitySimpleCacheOnlyDontNewUse> EntitySimpleCacheOnlyDontNewUses { get; set; }

            public DbSet<Blog> Blogs { get; set; }
			public DbSet<Post> Posts { get; set; }
			public DbSet<Comment> Comments { get; set; }
		}

        public class EntitySimpleCacheOnlyDontNewUse
        {
            public int Id { get; set; }
            public int ColumnInt { get; set; }
            public string ColumnString { get; set; }
        }
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


}
