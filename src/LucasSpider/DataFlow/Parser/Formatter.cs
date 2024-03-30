using System;

namespace LucasSpider.DataFlow.Parser
{
	/// <summary>
	/// Data formatting
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public abstract class Formatter : Attribute
	{
		/// <summary>
		/// Construction method
		/// </summary>
		protected Formatter()
		{
			Name = GetType().Name;
		}

		/// <summary>
		/// Formatted name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Return value if the formatted value is empty
		/// </summary>
		public string Default { get; set; }

		/// <summary>
		/// Achieve numerical conversion
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>The formatted value</returns>
		protected abstract string Handle(string value);

		/// <summary>
		/// Verify whether the parameters are set correctly
		/// </summary>
		protected abstract void CheckArguments();

		/// <summary>
		/// Format data
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>The formatted value</returns>
		public string Format(string value)
		{
			CheckArguments();

			return value == default ? Default : Handle(value);
		}
	}
}
