using System.Reflection;

namespace LucasSpider.DataFlow.Storage
{
	/// <summary>
	/// Column information
	/// </summary>
	public class Column
	{
		public string Name { get; set; }
		public string Type { get; set; }
		public int Length { get; set; } = 255;
		public bool Required { get; set; }

		/// <summary>
		/// Property reflection, used to set parsed values ​​to entity objects
		/// </summary>
		public PropertyInfo PropertyInfo { get; set; }
	}
}
