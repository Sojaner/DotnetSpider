using System;

namespace LucasSpider.DataFlow.Parser.Formatters
{
	/// <summary>
	/// Convert Unix time to DateTime
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class TimeStampFormatter : Formatter
	{
		/// <summary>
		/// Achieve numerical conversion
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>The formatted value</returns>
		protected override string Handle(string value)
		{
			var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			var tmp = value;
			if (!long.TryParse(tmp, out var timeStamp))
			{
				return dt.ToString("yyyy-MM-dd HH:mm:ss");
			}

			switch (tmp.Length)
			{
				case 10:
					{
						dt = dt.AddSeconds(timeStamp).ToLocalTime();
						break;
					}
				case 13:
					{
						dt = dt.AddMilliseconds(timeStamp).ToLocalTime();
						break;
					}
				default:
					{
						throw new ArgumentException("Wrong input timestamp");
					}
			}
			return dt.ToString("yyyy-MM-dd HH:mm:ss");
		}

		/// <summary>
		/// Verify whether the parameters are set correctly
		/// </summary>
		protected override void CheckArguments()
		{
		}
	}
}
