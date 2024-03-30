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
	public class UrlEncodeFormatter : Formatter
	{
		/// <summary>
		/// Encoding name
		/// </summary>
		public string Encoding { get; set; }

		/// <summary>
		/// Achieve numerical conversion
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>The formatted value</returns>
		protected override string Handle(string value)
		{
			var tmp = value;
#if !NETSTANDARD
			return HttpUtility.UrlEncode(tmp, System.Text.Encoding.GetEncoding(Encoding));
#else
			return WebUtility.UrlEncode(tmp);
#endif
		}

		/// <summary>
		/// Verify whether the parameters are set correctly
		/// </summary>
		protected override void CheckArguments()
		{
			var encoding = System.Text.Encoding.GetEncoding(Encoding);
			if (encoding == null)
			{
				throw new ArgumentException($"Can't get encoding: {Encoding}");
			}
		}
	}
}
