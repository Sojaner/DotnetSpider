using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LucasSpider.Selector
{
    /// <summary>
    /// Query interface
    /// </summary>
    public abstract class Selectable : ISelectable
    {
        /// <summary>
        /// Find all links
        /// </summary>
        /// <returns>Query interface</returns>
        public abstract IEnumerable<string> Links();

        public abstract SelectableType Type { get; }

        /// <summary>
        /// Find results via XPath
        /// </summary>
        /// <param name="xpath">XPath expression</param>
        /// <returns>Query interface</returns>
        public virtual ISelectable XPath(string xpath)
        {
            return Select(Selectors.XPath(xpath));
        }

        /// <summary>
        /// Find elements through CSS selectors and get attribute values
        /// </summary>
        /// <param name="css">Css selector</param>
        /// <param name="attrName">Attributes of the queried element</param>
        /// <returns>Query interface</returns>
        public ISelectable Css(string css, string attrName)
        {
            return Select(Selectors.Css(css, attrName));
        }

        /// <summary>
        /// Find results via JsonPath
        /// </summary>
        /// <param name="jsonPath">JsonPath expression</param>
        /// <returns>Query interface</returns>
        public virtual ISelectable JsonPath(string jsonPath)
        {
            return Select(Selectors.JsonPath(jsonPath));
        }

        /// <summary>
        /// Find results by regular expression
        /// </summary>
        /// <param name="pattern">regular expression</param>
        /// <param name="options"></param>
        /// <param name="replacement"></param>
        /// <returns>Query interface</returns>
        public virtual ISelectable Regex(string pattern, RegexOptions options = RegexOptions.None, string replacement = "$0")
        {
            return Select(Selectors.Regex(pattern, options, replacement));
        }

        public abstract IEnumerable<ISelectable> Nodes();

        public abstract string Value { get; }

        /// <summary>
        /// Find results via query
        /// </summary>
        /// <param name="selector">Queryer</param>
        /// <returns>Query interface</returns>
        public abstract ISelectable Select(ISelector selector);

        /// <summary>
        /// Find results via query
        /// </summary>
        /// <param name="selector">Queryer</param>
        /// <returns>Query interface</returns>
        public abstract IEnumerable<ISelectable> SelectList(ISelector selector);
    }
}
