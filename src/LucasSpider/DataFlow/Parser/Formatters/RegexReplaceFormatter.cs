using System;
using System.Text.RegularExpressions;

namespace LucasSpider.DataFlow.Parser.Formatters
{
	/// <summary>
	///  In a specified input string, replaces all strings that match a specified regular expression with a specified replacement string.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class RegexReplaceFormatter : Formatter
	{
		/// <summary>
		/// Regular expression
		/// </summary>
		public string Pattern { get; set; }

		/// <summary>
		/// The replacement string
		/// </summary>
		public string NewValue{ get; set; }

		/// <summary>
		/// Achieve numerical conversion
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>The formatted value</returns>
		protected override string Handle(string value)
		{
			return Regex.Replace(value, Pattern, NewValue);
		}

		/// <summary>
		/// Verify whether the parameters are set correctly
		/// </summary>
		protected override void CheckArguments()
		{
			if (string.IsNullOrWhiteSpace(Pattern))
			{
				throw new ArgumentException("Pattern should not be null or empty");
			}
		}
	}
}
