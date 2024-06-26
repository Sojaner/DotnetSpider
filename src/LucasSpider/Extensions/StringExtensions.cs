﻿using System;
using Newtonsoft.Json.Linq;

namespace LucasSpider.Extensions
{
	internal static class StringExtensions
	{
		public static bool TryParseJToken(this string jsonString, out JToken token)
		{
			try
			{
				token = JToken.Parse(jsonString);
				return true;
			}
			catch (Exception)
			{
				token = null;
				return false;
			}
		}

		public static bool Contains(this string source, string value, StringComparison comparison)
		{
#if NETSTANDARD2_0
			return source.IndexOf(value, comparison) >= 0;
#else
			return source.Contains(value, comparison);
#endif
		}

		public static bool TryParseXmlDocument(this string xmlString, out System.Xml.XmlDocument xmlDocument)
		{
			try
			{
				xmlDocument = new System.Xml.XmlDocument();
				xmlDocument.LoadXml(xmlString);
				return true;
			}
			catch (Exception)
			{
				xmlDocument = null;
				return false;
			}
		}
	}
}
