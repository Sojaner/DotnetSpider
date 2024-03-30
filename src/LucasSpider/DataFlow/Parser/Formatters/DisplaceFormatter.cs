using System;

namespace LucasSpider.DataFlow.Parser.Formatters
{
	/// <summary>
	/// If the value is equal to EqualValue, then Displacement is returned. For example, used for: The collected result is: Yes, converted to False
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class DisplaceFormatter : Formatter
	{
		/// <summary>
		/// Compare value
		/// </summary>
		public string EqualValue { get; set; }

		/// <summary>
		/// Final replaced value
		/// </summary>
		public string Displacement { get; set; }

		/// <summary>
		/// Achieve numerical conversion
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>The formatted value</returns>
		protected override string Handle(string value)
		{
			return value.Equals(EqualValue) ? Displacement : value;
		}

		/// <summary>
		/// Verify whether the parameters are set correctly
		/// </summary>
		protected override void CheckArguments()
		{
		}
	}
}
