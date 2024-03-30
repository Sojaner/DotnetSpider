using System;
using System.Text.RegularExpressions;

namespace LucasSpider.DataFlow.Parser.Formatters
{
	/// <summary>
	/// Convert strings containing Chinese characters into numbers
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class DigitUnitFormatter : Formatter
	{
		private const string UnitStringForShi = "十";
		private const string UnitStringForBai = "百";
		private const string UnitStringForQian = "千";
		private const string UnitStringForWan = "万";
		private const string UnitStringForYi = "亿";
		private readonly Regex _decimalRegex = new(@"\d+(\.\d+)?");

		/// <summary>
		/// Digital formatting template
		/// </summary>
		public string NumberFormat { get; set; } = "F0";

		/// <summary>
		/// Convert strings containing Chinese characters into numbers
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>The formatted value</returns>
		protected override string Handle(string value)
		{
			var tmp = value;
			var num = decimal.Parse(_decimalRegex.Match(tmp).ToString());
			if (tmp.EndsWith(UnitStringForShi))
			{
				num = num * 10;
			}
			else if (tmp.EndsWith(UnitStringForBai))
			{
				num = num * 100;
			}
			else if (tmp.EndsWith(UnitStringForBai))
			{
				num = num * 100;
			}
			else if (tmp.EndsWith(UnitStringForQian))
			{
				num = num * 1000;
			}
			else if (tmp.EndsWith(UnitStringForWan))
			{
				num = num * 10000;
			}
			else if (tmp.EndsWith(UnitStringForYi))
			{
				num = num * 100000000;
			}
			return num.ToString(NumberFormat);
		}

		/// <summary>
		/// Verify whether the parameters are set correctly
		/// </summary>
		protected override void CheckArguments()
		{
		}
	}
}
