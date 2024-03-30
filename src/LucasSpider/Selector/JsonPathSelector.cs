using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LucasSpider.Selector
{
	/// <summary>
	/// JsonPath selector.
	/// Used to extract content from JSON.
	/// </summary>
	public class JsonPathSelector : ISelector
	{
		private readonly string _jsonPath;

		/// <summary>
		/// Construction method
		/// </summary>
		/// <param name="jsonPath">JsonPath</param>
		public JsonPathSelector(string jsonPath)
		{
			_jsonPath = jsonPath;
		}

		/// <summary>
		/// Query a single result from JSON text
		/// If there are multiple results that meet the criteria, only the first one is returned
		/// </summary>
		/// <param name="text">Json text to be queried</param>
		/// <returns>Query results</returns>
		public ISelectable Select(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				return null;
			}

			if (!(JsonConvert.DeserializeObject(text) is JToken token))
			{
				return null;
			}

			var result = token.SelectToken(_jsonPath);
			return result == null ? null : new JsonSelectable(result);
		}

		/// <summary>
		/// Query all results from JSON text
		/// </summary>
		/// <param name="text">Json text to be queried</param>
		/// <returns>Query results</returns>
		public IEnumerable<ISelectable> SelectList(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				return null;
			}

			if (!(JsonConvert.DeserializeObject(text) is JToken token))
			{
				return Enumerable.Empty<ISelectable>();
			}

			return token.SelectTokens(_jsonPath)
				.Select(x => new JsonSelectable(x));
		}
	}
}
