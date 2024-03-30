using System;
using System.Text.RegularExpressions;
using LucasSpider.Selector;

namespace LucasSpider.DataFlow.Parser
{
    public static class SelectorExtensions
    {
        /// <summary>
        /// Convert BaseSelector into a real queryer
        /// </summary>
        /// <param name="selector">BaseSelector</param>
        /// <returns>Queryer</returns>
        public static ISelector ToSelector(this Selector selector)
        {
            if (selector != null)
            {
                var expression = selector.Expression;

                switch (selector.Type)
                {
                    case SelectorType.Css:
                    {
                        NotNullExpression(selector);
                        return Selectors.Css(expression);
                    }
                    case SelectorType.JsonPath:
                    {
                        NotNullExpression(selector);
                        return Selectors.JsonPath(expression);
                    }
                    case SelectorType.Regex:
                    {
                        NotNullExpression(selector);
                        if (string.IsNullOrEmpty(selector.Arguments))
                        {
                            return Selectors.Regex(expression);
                        }

                        var arguments = selector.Arguments.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                        var options = (RegexOptions) Enum.Parse(typeof(RegexOptions), arguments[0]);
                        var replacement = arguments[1];
                        return Selectors.Regex(expression, options, replacement);
                    }
                    case SelectorType.XPath:
                    {
                        NotNullExpression(selector);
                        return Selectors.XPath(expression);
                    }
                    default:
                    {
                        throw new NotSupportedException($"{selector} unsupported");
                    }
                }
            }

            return null;
        }

        private static void NotNullExpression(Selector selector)
        {
            if (string.IsNullOrWhiteSpace(selector.Expression))
            {
                throw new ArgumentException($"Expression of {selector} should not be null/empty");
            }
        }
    }
}
