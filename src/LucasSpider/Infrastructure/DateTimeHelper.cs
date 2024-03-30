using System;

namespace LucasSpider.Infrastructure
{
	public static class DateTimeHelper
	{
		/// <summary>
		/// The first day of the current month
		/// </summary>
		public static DateTime FirstDayOfMonth
		{
			get
			{
				var now = DateTime.Now.Date;
				return now.AddDays(now.Day * -1 + 1);
			}
		}

		/// <summary>
		/// The last day of the current month
		/// </summary>
		public static DateTime LastDayOfMonth => FirstDayOfMonth.AddMonths(1).AddDays(-1);

		/// <summary>
		/// The first day of the previous month
		/// </summary>
		public static DateTime FirstDayOfLastMonth => FirstDayOfMonth.AddMonths(-1);

		/// <summary>
		/// The last day of the previous month
		/// </summary>
		public static DateTime LastDayOfLastMonth => FirstDayOfMonth.AddDays(-1);

		/// <summary>
		/// That day
		/// </summary>
		public static DateTime Today => DateTime.Now.Date;

		/// <summary>
		/// Monday
		/// </summary>
		public static DateTime Monday
		{
			get
			{
				var now = DateTime.Now;
				var i = now.DayOfWeek - DayOfWeek.Monday == -1 ? 6 : -1;
				var ts = new TimeSpan(i, 0, 0, 0);

				return now.Subtract(ts).Date;
			}
		}

		/// <summary>
		/// Tuesday
		/// </summary>
		public static DateTime Tuesday => Monday.AddDays(1);

		/// <summary>
		/// Wednesday
		/// </summary>
		public static DateTime Wednesday => Monday.AddDays(2);

		/// <summary>
		/// Thursday
		/// </summary>
		public static DateTime Thursday => Monday.AddDays(3);

		/// <summary>
		/// Friday
		/// </summary>
		public static DateTime Friday => Monday.AddDays(4);

		/// <summary>
		/// Saturday
		/// </summary>
		public static DateTime Saturday => Monday.AddDays(5);

		/// <summary>
		/// Sunday
		/// </summary>
		public static DateTime Sunday => Monday.AddDays(6);
	}
}
