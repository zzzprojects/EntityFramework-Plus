using System.Collections.Generic;

namespace Z.EF.Plus.BatchUpdate.Shared.Extensions
{
	public static partial class DbModelPlusExtentions
	{
		public static TValue GetValueOrNull<TKey, TValue>(this Dictionary<TKey, TValue> @this, TKey key) where TValue : class
		{
			if (@this.ContainsKey(key))
			{
				return @this[key];
			}

			return null;
		}
	}
}