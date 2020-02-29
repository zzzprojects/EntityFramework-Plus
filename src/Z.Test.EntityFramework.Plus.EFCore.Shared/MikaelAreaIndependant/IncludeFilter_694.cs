using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus.EFCore.Shared.MikaelAreaIndependant
{
	[TestClass]
	public class IncludeFilter_694
	{
        public static void GenerateData()
		{

			using (var context = new ModelAndContext.EntityContext())
			{
				context.Blogs.Add(new Blog() { Title = "Blog_A", IsSoftDeleted = true });
				context.Blogs.Add(new Blog() { Title = "Blog_B", IsSoftDeleted = true });

				context.SaveChanges();

				context.Posts.Add(new Post() { Content = "Post 1 in Blog_A", Date = DateTime.Now.AddDays(-4), IsSoftDeleted = false, Price = 35, BlogId = 1 });
				context.Posts.Add(new Post() { Content = "Post 2 in Blog_A", Date = DateTime.Now.AddDays(-4), IsSoftDeleted = true, Price = 22, BlogId = 1 });
				context.Posts.Add(new Post() { Content = "Post 1 in Blog_B", Date = DateTime.Now.AddDays(-5), IsSoftDeleted = false, Price = 37, BlogId = 2 });
				context.Posts.Add(new Post() { Content = "Post 2 in Blog_B", Date = DateTime.Now.AddDays(-5), IsSoftDeleted = false, Price = 7, BlogId = 2 });

				context.SaveChanges();

				context.Comments.Add(new Comment() { Text = "Comment 1 in post 1", IsSoftDeleted = false, PostId = 1 });
				context.Comments.Add(new Comment() { Text = "Comment 2 in post 1", IsSoftDeleted = false, PostId = 1 });
				context.Comments.Add(new Comment() { Text = "Comment 1 in post 3", IsSoftDeleted = true, PostId = 3 });
				context.Comments.Add(new Comment() { Text = "Comment 2 in post 3", IsSoftDeleted = false, PostId = 3 });

				context.SaveChanges();
			}
		}

		public static void CleanData()
		{
			using (var context = new ModelAndContext.EntityContext())
			{
				context.Comments.Delete();
				context.Blogs.Delete();
				context.Posts.Delete();
			}
		}

		[TestMethod()]
		public void Select()
		{
			CleanData();
			GenerateData();

			using (var context = new ModelAndContext.EntityContext())
			{
				// LOAD blogs and related active posts and comments.
				var list = context.Blogs.IncludeFilter(x => x.Posts.Where(y => !y.IsSoftDeleted))
					.IncludeFilter(x => x.Posts.Where(y => !y.IsSoftDeleted)
						.Select(y => y.Comments.Where(z => !z.IsSoftDeleted)))
					.ToList();
			}
			
		}

		[TestMethod()]
		public void SelectMany()
		{
			CleanData();
			GenerateData();

			using (var context = new ModelAndContext.EntityContext())
			{
				// LOAD blogs and related active posts and comments.
				var list = context.Blogs.IncludeFilter(x => x.Posts.Where(y => !y.IsSoftDeleted))
					.IncludeFilter(x => x.Posts.Where(y => !y.IsSoftDeleted)
						.SelectMany(y => y.Comments.Where(z => !z.IsSoftDeleted)))
					.ToList();
			}
		}
	}
}
