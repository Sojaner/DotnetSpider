using System;
using System.Linq;

namespace LucasSpider.DataFlow.Parser.Formatters
{
	/// <summary>
	/// Splits a string into substrings based on the strings in an array. You can specify whether the substrings include empty array elements.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class SplitFormatter : Formatter
	{
		/// <summary>
		///  A string array that delimits the substrings in this string, an empty array that contains no delimiters, or null.
		/// </summary>
		public string[] Separator { get; set; }

		/// <summary>
		/// The index of the value that needs to be returned after splitting the value
		/// </summary>
		public int ElementAt { get; set; } = int.MaxValue;

		/// <summary>
		/// Achieve numerical conversion
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>The formatted value</returns>
		protected override string Handle(string value)
		{
			var result = value.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

			if (result.Length > ElementAt)
			{
				return result[ElementAt];
			}

			return ElementAt == int.MaxValue ? result.Last() : null;
		}

		/// <summary>
		/// Verify whether the parameters are set correctly
		/// </summary>
		protected override void CheckArguments()
		{
			if (Separator == null || Separator.Length == 0)
			{
				throw new ArgumentException("Separator should not be null or empty");
			}

			if (ElementAt < 0)
			{
				throw new ArgumentException("ElementAt should larger than 0");
			}
		}
	}
}
