namespace LucasSpider.DataFlow.Storage
{
	/// <summary>
	/// Table name suffix
	/// </summary>
	public enum TablePostfix
	{
		/// <summary>
		/// None
		/// </summary>
		None,

		/// <summary>
		/// The suffix of the table name is Monday's time
		/// </summary>
		Monday,

		/// <summary>
		/// The suffix of the table name is today’s time {name}_20171212
		/// </summary>
		Today,

		/// <summary>
		/// The suffix of the table name is {name}_201712 for the current month
		/// </summary>
		Month
	}
}
