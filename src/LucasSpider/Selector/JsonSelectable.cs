using System.Collections.Generic;
using System.Linq;
using LucasSpider.Infrastructure;
using Newtonsoft.Json.Linq;

namespace LucasSpider.Selector
{
    public class JsonSelectable : Selectable
    {
        private readonly JToken _token;

        public JsonSelectable(JToken token)
        {
            _token = token;
        }

        public override IEnumerable<string> Links()
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<ISelectable> Nodes()
        {
            return _token.Children().Select(x => new JsonSelectable(x));
        }

        public override string Value => _token?.ToString();

        /// <summary>
        /// Find results via query
        /// </summary>
        /// <param name="selector">Queryer</param>
        /// <returns>Query interface</returns>
        public override ISelectable Select(ISelector selector)
        {
            selector.NotNull(nameof(selector));
            return selector.Select(_token.ToString());
        }

        /// <summary>
        /// Find results via query
        /// </summary>
        /// <param name="selector">Queryer</param>
        /// <returns>Query interface</returns>
        public override IEnumerable<ISelectable> SelectList(ISelector selector)
        {
            selector.NotNull(nameof(selector));
            return selector.SelectList(_token.ToString());
        }

        public override SelectableType Type => SelectableType.Json;
    }
}
