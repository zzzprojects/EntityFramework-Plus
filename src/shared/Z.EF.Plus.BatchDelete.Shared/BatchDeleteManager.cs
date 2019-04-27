// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
#if EFCORE
using Microsoft.EntityFrameworkCore;
#endif

namespace Z.EntityFramework.Plus
{
	/// <summary>Manage EF+ Batch Delete Configuration.</summary>
	public class BatchDeleteManager
	{
		/// <summary>Gets or sets the batch delete builder to change default configuration.</summary>
		/// <value>The batch delete builder to change default configuration.</value>
		public static Action<BatchDelete> BatchDeleteBuilder { get; set; }

#if EFCORE

		/// <summary>Gets or sets the factory to create an InMemory DbContext.</summary>
		/// <value>The factory to create an InMemory DbContext.</value>
		public static Func<DbContext> InMemoryDbContextFactory { get; set; }

		internal static DbContext CreateFactoryContext(DbContext context )
		{
			if (InMemoryDbContextFactory != null)
			{
				var newContext = InMemoryDbContextFactory();

				if (newContext != null)
				{
					if (newContext.GetType() == context.GetType())
					{
						var optionsField = typeof(DbContext).GetField("_options",
							BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
						var originalOptions = optionsField.GetValue(context);
						optionsField.SetValue(newContext, originalOptions);
					}

					EnsureContextSimilar(context, newContext);
					return newContext;
				}
			}

			var type = context.GetType();
			var emtptyConstructor = type.GetConstructor(new Type[0]);

			if (emtptyConstructor != null)
			{
				var newContext = (DbContext) emtptyConstructor.Invoke(new object[0]);

				var optionsField = typeof(DbContext).GetField("_options",
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				var originalOptions = optionsField.GetValue(context);
				optionsField.SetValue(newContext, originalOptions);

				EnsureContextSimilar(context, newContext);
				return newContext;
			}

			throw new Exception(
				"A default DbContext context must exist, or a context factory must be provided (BatchDeleteManager.BatchDeleteBuilder). This setting is required for some additional features. Read more: http://entityframework-extensions.net/context-factory");
		}

		internal static void EnsureContextSimilar(DbContext originalContext, DbContext newContext)
		{
			if (originalContext.IsInMemory() && !newContext.IsInMemory())
			{
				throw new Exception(
					"Oops! The original context was an “InMemory” provider. Make sure that the method BatchDeleteManager.BatchDeleteBuilder returns an InMemory provider.");
			}
		}

#else
        /// <summary>
        /// Gets or sets a value indicating whether this object is in memory query.
        /// </summary>
        /// <value>True if this object is in memory query, false if not.</value>
        public static bool IsInMemoryQuery { get; set; }
#endif
	}
}
