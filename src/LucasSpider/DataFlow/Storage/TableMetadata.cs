using System.Collections.Generic;
using System.Linq;

namespace LucasSpider.DataFlow.Storage
{
	/// <summary>
	/// Table metadata
	/// </summary>
	public class TableMetadata
	{
		/// <summary>
		/// Entity type name
		/// </summary>
		public string TypeName { get; set; }

		/// <summary>
		/// Schema
		/// </summary>
		public Schema Schema { get; set; }

		/// <summary>
		/// Primary key
		/// </summary>
		public HashSet<string> Primary { get; set; }

		/// <summary>
		/// Index
		/// </summary>
		public HashSet<IndexMetadata> Indexes { get; }

		/// <summary>
		/// Update column
		/// </summary>
		public HashSet<string> Updates { get; set; }

		/// <summary>
		/// Attribute name, dictionary of attribute data types
		/// </summary>
		public Dictionary<string, Column> Columns { get; }

		/// <summary>
		/// Whether it is an auto-incrementing primary key
		/// </summary>
		public bool IsAutoIncrementPrimary => Primary != null && Primary.Count == 1 &&
		                                      (Columns[Primary.First()].Type == "Int32" ||
		                                       Columns[Primary.First()].Type == "Int64");

		/// <summary>
		/// Determine whether a column is in the primary key
		/// </summary>
		/// <param name="column">column</param>
		/// <returns></returns>
		public bool IsPrimary(string column)
		{
			return Primary != null && Primary.Contains(column);
		}

		/// <summary>
		/// Determine whether there is a primary key
		/// </summary>
		public bool HasPrimary => Primary != null && Primary.Count > 0;

		/// <summary>
		/// Determine whether there is an updated column
		/// </summary>
		public bool HasUpdateColumns => Updates != null && Updates.Count > 0;

		/// <summary>
		/// Construction method
		/// </summary>
		public TableMetadata()
		{
			Indexes = new HashSet<IndexMetadata>();
			Columns = new Dictionary<string, Column>();
			Primary = new HashSet<string>();
			Updates = new HashSet<string>();
		}
	}
}
