using System;
using System.Text.RegularExpressions;

namespace LucasSpider.DataFlow.Parser.Formatters
{
	/// <summary>
	/// If the value matches the regular expression, append the specified content after the value. If the collected content is a number, add "person" after it.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class RegexAppendFormatter : Formatter
	{
		/// <summary>
		/// Regular expression
		/// </summary>
		public string Pattern { get; set; }

		/// <summary>
		/// Additional content
		/// </summary>
		public string AppendValue { get; set; }

		/// <summary>
		/// Achieve numerical conversion
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>The formatted value</returns>
		protected override string Handle(string value)
		{
			var tmp = value;
			return Regex.IsMatch(tmp, Pattern) ? $"{tmp}{AppendValue}" : tmp;
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

			if (string.IsNullOrWhiteSpace(AppendValue))
			{
				throw new ArgumentException("Append should not be null or empty");
			}
		}
	}
}
