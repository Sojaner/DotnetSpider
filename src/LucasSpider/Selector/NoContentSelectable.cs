﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LucasSpider.Selector
{
	public class NoContentSelectable: ISelectable
	{
		public object Clone()
		{
			return new NoContentSelectable();
		}

		public SelectableType Type => SelectableType.NoContent;

		public ISelectable XPath(string xpath)
		{
			throw new NotImplementedException();
		}

		public ISelectable Css(string css, string attr = null)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<string> Links()
		{
			throw new NotImplementedException();
		}

		public ISelectable JsonPath(string jsonPath)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ISelectable> Nodes()
		{
			throw new NotImplementedException();
		}

		public ISelectable Regex(string pattern, RegexOptions options = RegexOptions.None, string replacement = "$0")
		{
			throw new NotImplementedException();
		}

		public string Value => null;

		public ISelectable Select(ISelector selector)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ISelectable> SelectList(ISelector selector)
		{
			throw new NotImplementedException();
		}
	}
}
