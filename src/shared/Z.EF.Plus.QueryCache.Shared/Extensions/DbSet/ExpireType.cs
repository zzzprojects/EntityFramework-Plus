using System.Data.Common;
using System.Reflection;
#if EFCORE
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
#elif EF6 || EF5 || EFCLASSIC
using System.Data.Entity;
#endif
namespace Z.EntityFramework.Plus
{
	public static partial class DbSetExtensions
	{
        /// <summary>A DbSet&lt;T&gt; extension method that expire cache.</summary>
        /// <param name="dbSet">The dbSet to act on.</param>
        public static void ExpireCache<T>(this DbSet<T> dbSet) where T : class
		{
			QueryCacheManager.ExpireType(typeof(T));
		}
	}
}  