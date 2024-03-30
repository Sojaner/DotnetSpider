using System;
using System.Net;

#if !NETSTANDARD
using System.Web;
#else

#endif

namespace LucasSpider.DataFlow.Parser.Formatters
{
	/// <summary>
	/// HTML decode the value
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class HtmlDecodeFormatter : Formatter
	{
		/// <summary>
		/// Achieve numerical conversion
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>The formatted value</returns>
		protected override string Handle(string value)
		{
			var tmp = value;
#if !NETSTANDARD
			return HttpUtility.HtmlDecode(tmp);
#else
			return WebUtility.HtmlDecode(tmp);
#endif
		}

		/// <summary>
		/// Verify whether the parameters are set correctly
		/// </summary>
		protected override void CheckArguments()
		{
		}
	}
}
