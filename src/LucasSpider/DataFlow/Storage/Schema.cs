using System;

namespace LucasSpider.DataFlow.Storage
{
	/// <summary>
	/// Schema information
	/// </summary>
	public class Schema : Attribute
	{
		/// <summary>
		/// Database name
		/// </summary>
		public string Database { get; }

		/// <summary>
		/// Table Name
		/// </summary>
		public string Table { get; }

		/// <summary>
		/// Table name suffix
		/// </summary>
		public TablePostfix TablePostfix { get; set; }

		/// <summary>
		/// Construction method
		/// </summary>
		/// <param name="database">Database name</param>
		/// <param name="table">Table name</param>
		/// <param name="tablePostfix">Table name suffix</param>
		public Schema(string database, string table, TablePostfix tablePostfix = TablePostfix.None)
		{
			Database = database;
			Table = table;
			TablePostfix = tablePostfix;
		}
	}
}
