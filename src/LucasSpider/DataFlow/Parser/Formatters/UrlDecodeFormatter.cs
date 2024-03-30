#if !NETSTANDARD
using System.Web;
#else
using System.Net;
#endif
using System;

namespace LucasSpider.DataFlow.Parser.Formatters
{
	/// <summary>
	/// Converts a text string into a URL-encoded string.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class UrlDecodeFormatter : Formatter
	{
		protected override void CheckArguments()
		{
		}

		/// <summary>
		/// Achieve numerical conversion
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>The formatted value</returns>
		protected override string Handle(string value)
		{
			var tmp = value;
#if !NETSTANDARD
			return HttpUtility.UrlDecode(tmp);
#else
			return WebUtility.UrlDecode(tmp);
#endif
		}

	}
}
