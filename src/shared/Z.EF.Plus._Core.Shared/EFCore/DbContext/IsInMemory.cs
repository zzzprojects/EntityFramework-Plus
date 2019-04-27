#if EFCORE
using System.Linq;
using Microsoft.EntityFrameworkCore; 

namespace Z.EntityFramework.Plus
{
	public static partial class DbContextExtensions
	{
		public static bool IsInMemory(this DbContext @this)
		{
			return @this.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";
		}
	}
}
#endif