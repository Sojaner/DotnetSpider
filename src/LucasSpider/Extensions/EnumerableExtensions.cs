using System.Collections.Generic;
using System.Linq;

namespace LucasSpider.Extensions
{
	public static class EnumerableExtensions
	{
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
		{
			return list == null || !list.Any();
		}
	}
}
