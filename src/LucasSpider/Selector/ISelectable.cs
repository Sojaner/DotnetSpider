using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LucasSpider.Selector
{
    /// <summary>
    /// Query interface
    /// </summary>
    public interface ISelectable
    {
        SelectableType Type { get; }

        /// <summary>
        /// Find results via XPath
        /// </summary>
        /// <param name="xpath">XPath expression</param>
        /// <returns>Query interface</returns>
        ISelectable XPath(string xpath);

        /// <summary>
        /// Find elements through CSS selectors and get attribute values
        /// </summary>
        /// <param name="css">Css selector</param>
        /// <param name="attr">Attributes of the queried element</param>
        /// <returns>Query interface</returns>
        ISelectable Css(string css, string attr = null);

        /// <summary>
        /// Find all links
        /// </summary>
        /// <returns>Query interface</returns>
        IEnumerable<string> Links();

        /// <summary>
        /// Find results via JsonPath
        /// </summary>
        /// <param name="jsonPath">JsonPath expression</param>
        /// <returns>Query interface</returns>
        ISelectable JsonPath(string jsonPath);

        IEnumerable<ISelectable> Nodes();

        /// <summary>
        /// Find results by regular expression
        /// </summary>
        /// <param name="pattern">regular expression</param>
        /// <param name="options"></param>
        /// <param name="group">Group</param>
        /// <returns>Query interface</returns>
        ISelectable Regex(string pattern, RegexOptions options = RegexOptions.None, string replacement = "$0");

        /// <summary>
        /// Get the text result of the current query. If there are multiple query results, return the value of the first result.
        /// </summary>
        /// <returns>Query text results</returns>
        string Value { get; }

        /// <summary>
        /// Find results via query
        /// </summary>
        /// <param name="selector">Queryer</param>
        /// <returns>Query interface</returns>
        ISelectable Select(ISelector selector);

        /// <summary>
        /// Find results via query
        /// </summary>
        /// <param name="selector">Queryer</param>
        /// <returns>Query interface</returns>
        IEnumerable<ISelectable> SelectList(ISelector selector);
    }
}
