using System.Collections.Generic;
using System.Linq;
using LucasSpider.HtmlAgilityPack.Css;
using HtmlAgilityPack;

namespace LucasSpider.Selector
{
    /// <summary>
    /// CSS selectors
    /// </summary>
    public class CssSelector : ISelector
    {
        private readonly string _selector;
        private readonly string _attrName;

        /// <summary>
        /// Construction method
        /// </summary>
        /// <param name="selector">Css selector</param>
        public CssSelector(string selector)
        {
            _selector = selector;
        }

        /// <summary>
        /// Construction method
        /// </summary>
        /// <param name="selector">Css selector</param>
        /// <param name="attr">Attribute name</param>
        public CssSelector(string selector, string attr)
        {
            _selector = selector;
            _attrName = attr;
        }

        /// <summary>
        /// Query the node, and the query result is the first element that meets the query conditions.
        /// </summary>
        /// <param name="text">HTML</param>
        /// <returns>Query results</returns>
        public ISelectable Select(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            var document = new HtmlDocument {OptionAutoCloseOnEnd = true};
            document.LoadHtml(text);
            var node = document.DocumentNode.QuerySelector(_selector);

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
        /// <param name="text">HTML</param>
        /// <returns>Query results</returns>
        public IEnumerable<ISelectable> SelectList(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }
            var document = new HtmlDocument {OptionAutoCloseOnEnd = true};
            document.LoadHtml(text);
            var nodes = document.DocumentNode.QuerySelectorAll(_selector);
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
    }
}
