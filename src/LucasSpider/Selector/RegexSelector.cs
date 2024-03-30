using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LucasSpider.Infrastructure;

namespace LucasSpider.Selector
{
    /// <summary>
    /// Regular query
    /// </summary>
    public class RegexSelector : ISelector
    {
        private readonly Regex _regex;
        private readonly string _replacement;

        /// <summary>
        /// Construction method
        /// </summary>
        /// <param name="pattern">regular expression</param>
        /// <param name="options"></param>
        /// <param name="replacement"></param>
        public RegexSelector(string pattern, RegexOptions options = RegexOptions.None, string replacement = "$0")
        {
            pattern.NotNullOrWhiteSpace(nameof(pattern));
            _regex = new Regex(pattern, options);
            _replacement = replacement;
        }

        /// <summary>
        /// Query a single result from text
        /// If there are multiple results that meet the criteria, only the first one is returned
        /// </summary>
        /// <param name="text">Text to be queried</param>
        /// <returns>Query results</returns>
        public ISelectable Select(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            var match = _regex.Match(text);
            if (match.Success)
            {
				return new TextSelectable(match.Result(_replacement));
            }

            return null;
        }

        /// <summary>
        /// Query all results from text
        /// </summary>
        /// <param name="text">Text to be queried</param>
        /// <returns>Query results</returns>
        public IEnumerable<ISelectable> SelectList(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            var matches = _regex.Matches(text);

            var results = new List<string>();
            foreach (Match match in matches)
            {
				var value = match.Result(_replacement);
				if (!string.IsNullOrWhiteSpace(value))
				{
					results.Add(value);
				}
            }

            return results.Select(x => new TextSelectable(x));
        }
    }
}
