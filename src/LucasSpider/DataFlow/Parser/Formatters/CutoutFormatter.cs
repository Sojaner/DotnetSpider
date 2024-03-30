using System;

namespace LucasSpider.DataFlow.Parser.Formatters
{
	/// <summary>
	/// Intercept value
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class CutoutFormatter : Formatter
	{
		/// <summary>
		/// Contents of the initial part
		/// </summary>
		public string StartPart { get; set; }

		/// <summary>
		/// The content of the closing part
		/// </summary>
		public string EndPart { get; set; }

		/// <summary>
		/// Offset to start interception
		/// </summary>
		public int StartOffset { get; set; } = 0;

		/// <summary>
		/// Offset to end interception
		/// </summary>
		public int EndOffset { get; set; } = 0;

		/// <summary>
		/// Achieve numerical conversion
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>The formatted value</returns>
		protected override string Handle(string value)
		{
			var tmp = value;
			var begin = tmp.IndexOf(StartPart, StringComparison.Ordinal);
			int length;
			if (!string.IsNullOrEmpty(EndPart))
			{
				var end = tmp.IndexOf(EndPart, begin, StringComparison.Ordinal);
				length = end - begin;
			}
			else
			{
				length = tmp.Length - begin;
			}

			begin += StartOffset;
			length -= StartOffset;
			length -= EndOffset;
			if (!string.IsNullOrEmpty(EndPart))
			{
				length += EndPart.Length;
			}

			return tmp.Substring(begin, length);
		}

		/// <summary>
		/// Verify whether the parameters are set correctly
		/// </summary>
		protected override void CheckArguments()
		{
		}
	}
}
