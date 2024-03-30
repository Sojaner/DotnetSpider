using System;
using System.Collections.Generic;
using System.Linq;
using LucasSpider.Infrastructure;

namespace LucasSpider.Selector
{
	/// <summary>
	/// Query interface
	/// </summary>
	public class TextSelectable : Selectable
	{
		private readonly string _text;

		public override SelectableType Type => SelectableType.Text;

		/// <summary>
		/// Construction method
		/// </summary>
		/// <param name="text">Content</param>
		public TextSelectable(string text)
		{
			_text = text;
		}

		/// <summary>
		/// Get all results in the query
		/// </summary>
		/// <returns>Query interface</returns>
		public override IEnumerable<ISelectable> Nodes()
		{
			return new[] {new TextSelectable(_text)};
		}

		/// <summary>
		/// Find all links
		/// </summary>
		/// <returns>Query interface</returns>
		public override IEnumerable<string> Links()
		{
			// todo: re-impl with regex
			var links = SelectList(Selectors.XPath("./descendant-or-self::*/@href")).Select(x => x.Value);
			var sourceLinks = SelectList(Selectors.XPath("./descendant-or-self::*/@src"))
				.Select(x => x.Value);

			var results = new HashSet<string>();
			foreach (var link in links)
			{
				if (Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out _))
				{
					results.Add(link);
				}
			}

			foreach (var link in sourceLinks)
			{
				if (Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out _))
				{
					results.Add(link);
				}
			}

			return results;
		}


		public override string Value => _text;

		/// <summary>
		/// Find results via query
		/// </summary>
		/// <param name="selector">Queryer</param>
		/// <returns>Query interface</returns>
		public override ISelectable Select(ISelector selector)
		{
			selector.NotNull(nameof(selector));
			return selector.Select(_text);
		}

		/// <summary>
		/// Find results via query
		/// </summary>
		/// <param name="selector">Queryer</param>
		/// <returns>Query interface</returns>
		public override IEnumerable<ISelectable> SelectList(ISelector selector)
		{
			selector.NotNull(nameof(selector));
			return selector.SelectList(_text);
		}
	}
}
