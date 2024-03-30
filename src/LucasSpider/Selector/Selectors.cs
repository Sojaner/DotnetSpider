using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace LucasSpider.Selector
{
    /// <summary>
    /// Query construction helper class, the same query will be cached.
    /// </summary>
    public class Selectors
    {
        private static readonly ConcurrentDictionary<string, ISelector> Cache =
            new();

        /// <summary>
        /// Create a regular query
        /// </summary>
        /// <param name="expr">regular expression</param>
        /// <param name="options"></param>
        /// <param name="group"></param>
        /// <returns>Queryer</returns>
        public static ISelector Regex(string expr,
            RegexOptions options = RegexOptions.None, string replacement = "$0")
        {
            var key = $"r_{expr}_{replacement}";
            if (!Cache.ContainsKey(key))
            {
                Cache.TryAdd(key, new RegexSelector(expr, options, replacement));
            }

            return Cache[key];
        }

        /// <summary>
        /// Create CSS query
        /// </summary>
        /// <param name="expr">Css expression</param>
        /// <param name="attr">Attribute name</param>
        /// <returns>Queryer</returns>
        public static ISelector Css(string expr, string attr = null)
        {
            var key = $"c_{expr}_{attr}";
            if (!Cache.ContainsKey(key))
            {
                Cache.TryAdd(key, new CssSelector(expr, attr));
            }

            return Cache[key];
        }

        /// <summary>
        /// Create an XPath query
        /// </summary>
        /// <param name="expr">Xpath expression</param>
        /// <returns>Queryer</returns>
        public static ISelector XPath(string expr)
        {
            var key = $"x_{expr}";
            if (!Cache.ContainsKey(key))
            {
                Cache.TryAdd(key, new XPathSelector(expr));
            }

            return Cache[key];
        }

        /// <summary>
        /// Create JsonPath queryer
        /// </summary>
        /// <param name="expr">JsonPath expression</param>
        /// <returns>Queryer</returns>
        public static ISelector JsonPath(string expr)
        {
            var key = $"j_{expr}";
            if (!Cache.ContainsKey(key))
            {
                Cache.TryAdd(key, new JsonPathSelector(expr));
            }

            return Cache[key];
        }
    }
}
