using System.Collections.Generic;

namespace LucasSpider.Selector
{
	/// <summary>
	/// Queryer
	/// </summary>
	public interface ISelector
	{
		/// <summary>
		/// Query a single result from text
		/// If there are multiple results that meet the criteria, only the first one is returned
		/// </summary>
		/// <param name="text">Text to be queried</param>
		/// <returns>Query results</returns>
		ISelectable Select(string text);

		/// <summary>
		/// Query all results from text
		/// </summary>
		/// <param name="text">Text to be queried</param>
		/// <returns>Query results</returns>
		IEnumerable<ISelectable> SelectList(string text);
	}
}
