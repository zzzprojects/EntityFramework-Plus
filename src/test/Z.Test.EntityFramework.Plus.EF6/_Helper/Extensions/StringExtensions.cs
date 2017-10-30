using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Z.Test.EntityFramework.Plus
{
	public static class StringExtensions
	{
		private static Regex _rNormalize = new Regex(@"\s+");

		public static string CollapseWhiteSpace(this string strSource)
		{
			return string.IsNullOrEmpty(strSource) ? strSource : _rNormalize.Replace(strSource, " ").Trim();
		}
	}
}
