using System;

namespace LucasSpider.DataFlow.Parser.Formatters
{
	/// <summary>
	/// Removes all leading and trailing white-space characters from the current System.String object.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class TrimFormatter : Formatter
	{
		/// <summary>
		/// Trim type
		/// </summary>
		public enum TrimType
		{
			/// <summary>
			/// Trim only the leading white-spaces
			/// </summary>
			Right,
			/// <summary>
			/// Trim only the trailing white-spaces
			/// </summary>
			Left,

			/// <summary>
			/// Trim both leading and trailing white-spaces
			/// </summary>
			All
		}

		/// <summary>
		/// Trim type
		/// </summary>
		public TrimType Type { get; set; } = TrimType.All;

		/// <summary>
		/// Achieve numerical conversion
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>The formatted value</returns>
		protected override string Handle(string value)
		{
			switch (Type)
			{
				case TrimType.All:
					{
						return value.Trim();
					}
				case TrimType.Left:
					{
						return value.TrimStart();
					}
				case TrimType.Right:
					{
						return value.TrimEnd();
					}
				default:
					{
						return value.Trim();
					}
			}
		}

		/// <summary>
		/// Verify whether the parameters are set correctly
		/// </summary>
		protected override void CheckArguments()
		{
		}
	}
}
