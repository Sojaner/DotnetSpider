using System;

namespace LucasSpider.DataFlow.Parser.Formatters
{
	/// <summary>
	/// Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class ReplaceFormatter : Formatter
	{
		/// <summary>
		/// The value that needs to be replaced
		/// </summary>
		public string OldValue { get; set; }

		/// <summary>
		/// The string to replace all occurrences of oldValue.
		/// </summary>
		public string NewValue { get; set; }

		/// <summary>
		/// Achieve numerical conversion
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>The formatted value</returns>
		protected override string Handle(string value)
		{
			return value.Replace(OldValue, NewValue);
		}

		/// <summary>
		/// Verify whether the parameters are set correctly
		/// </summary>
		protected override void CheckArguments()
		{
		}
	}
}
