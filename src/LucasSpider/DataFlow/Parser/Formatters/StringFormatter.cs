using System;

namespace LucasSpider.DataFlow.Parser.Formatters
{
	/// <summary>
	/// Replaces one or more format items in a specified string with the string representation of a specified object.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class StringFormatter : Formatter
	{
		/// <summary>
		/// A composite format string.
		/// </summary>
		public string FormatStr { get; set; }

		/// <summary>
		/// Achieve numerical conversion
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>The formatted value</returns>
		protected override string Handle(string value)
		{
			return string.Format(FormatStr, value);
		}

		/// <summary>
		/// Verify whether the parameters are set correctly
		/// </summary>
		protected override void CheckArguments()
		{
			if (string.IsNullOrWhiteSpace(FormatStr))
			{
				throw new ArgumentException("FormatString should not be null or empty");
			}
		}
	}
}
