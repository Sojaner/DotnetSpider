using System;
using System.Text.RegularExpressions;

namespace LucasSpider.DataFlow.Parser.Formatters
{
	/// <summary>
	/// If it matches the regular expression, it returns True. If it does not match the regular expression, it returns False.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class RegexFormatter : Formatter
	{
		private const string Id = "227a207a28024b1cbee3754e76443df2";

		/// <summary>
		/// Regular expression formatting
		/// </summary>
		public string Pattern { get; set; }

		/// <summary>
		/// What the regular expression should return
		/// </summary>
		public string True { get; set; } = Id;

		/// <summary>
		/// Does not match what the regular expression should return
		/// </summary>
		public string False { get; set; } = Id;

		/// <summary>
		/// If True is not set, the Group content matching the regular expression is returned.
		/// </summary>
		public int Group { get; set; } = -1;

		/// <summary>
		/// Achieve numerical conversion
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>The formatted value</returns>

		protected override string Handle(string value)
		{
			var tmp = value;
			var matches = Regex.Matches(tmp, Pattern);
			if (matches.Count > 0)
			{
				if (True == Id)
				{
					return Group < 0 ? matches[0].Value : matches[0].Groups[Group].Value;
				}

				return True;
			}

			return False == Id ? "" : False;
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
