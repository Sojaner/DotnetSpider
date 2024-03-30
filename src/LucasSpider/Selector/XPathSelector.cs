using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace LucasSpider.Selector
{
    /// <summary>
    /// Xpath query
    /// </summary>
    public class XPathSelector : ISelector
    {
        private static readonly Regex AttributeXPathRegex =
            new(@"@[\w\s-]+", RegexOptions.RightToLeft | RegexOptions.IgnoreCase);

        private readonly string _xpath;
        private readonly string _attrName;

        /// <summary>
        /// Construction method
        /// </summary>
        /// <param name="xpath">Xpath expression</param>
        public XPathSelector(string xpath)
        {
            _xpath = xpath;

            var match = AttributeXPathRegex.Match(_xpath);
            if (!string.IsNullOrWhiteSpace(match.Value) && _xpath.EndsWith(match.Value))
            {
                _attrName = match.Value.Replace("@", "");
                _xpath = _xpath.Replace("/" + match.Value, "");
            }
        }

        /// <summary>
        /// Query the node, and the query result is the first element that meets the query conditions.
        /// </summary>
        /// <param name="text">HTML element</param>
        /// <returns>Query results</returns>
        public ISelectable Select(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            var document = new HtmlDocument {OptionAutoCloseOnEnd = true};
            document.LoadHtml(text);
            var node = document.DocumentNode.SelectSingleNode(_xpath);

            if (node == null)
            {
                return null;
            }

            if (HasAttribute)
            {
                return new TextSelectable(node.Attributes[_attrName]?.Value?.Trim());
            }
            else
            {
                return new HtmlSelectable(node);
            }
        }

        /// <summary>
        /// Query the node, and the query result is all elements that meet the query conditions.
        /// </summary>
        /// <param name="text">HTML element</param>
        /// <returns>Query results</returns>
        public IEnumerable<ISelectable> SelectList(string text)
        {
            var document = new HtmlDocument {OptionAutoCloseOnEnd = true};
            document.LoadHtml(text);
            var nodes = document.DocumentNode.SelectNodes(_xpath);
            if (nodes == null)
            {
                return null;
            }

            if (HasAttribute)
            {
                return nodes.Select(x => new TextSelectable(x.Attributes[_attrName]?.Value?.Trim()));
            }
            else
            {
                return nodes.Select(node => new HtmlSelectable(node));
            }
        }

        /// <summary>
        /// Determine whether the query contains attributes
        /// </summary>
        /// <returns>If True is returned, it means the attribute value of the query element</returns>
        public bool HasAttribute => !string.IsNullOrWhiteSpace(_attrName);

        public override int GetHashCode()
        {
            return $"{_xpath}{_attrName}".GetHashCode();
        }
    }
}
