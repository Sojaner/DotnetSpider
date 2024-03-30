using System;

namespace LucasSpider.DataFlow.Parser.Formatters
{
	/// <summary>
	/// Capitalize or lowercase strings
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class CharacterCaseFormatter : Formatter
	{
		/// <summary>
		/// If True, the data will be uppercase. If False, the table data will be lowercase.
		/// </summary>
		public bool ToUpper { get; set; } = true;

		/// <summary>
		/// Achieve numerical conversion
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>The formatted value</returns>
		protected override string Handle(string value)
		{
			return ToUpper ? value.ToUpperInvariant() : value.ToLowerInvariant();
		}

		/// <summary>
		/// Verify whether the parameters are set correctly
		/// </summary>
		protected override void CheckArguments()
		{
		}
	}
}
