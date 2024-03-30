using System;
using System.Reflection;
using LucasSpider.Selector;

namespace LucasSpider.DataFlow.Parser
{
	/// <summary>
	/// Definition of attribute selector
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class ValueSelector : Selector
	{
		/// <summary>
		/// Property reflection, used to set parsed values ​​to entity objects
		/// </summary>
		internal PropertyInfo PropertyInfo { get; set; }

		/// <summary>
		/// Whether the value can be empty, if it cannot be empty but the parsed value is empty, the current object is discarded
		/// </summary>
		internal bool NotNull { get; set; }

		/// <summary>
		/// Construction method
		/// </summary>
		public ValueSelector()
		{
		}

		/// <summary>
		/// Construction method
		/// </summary>
		/// <param name="type">Selector type</param>
		/// <param name="expression">Expression</param>
		public ValueSelector(string expression, SelectorType type = SelectorType.XPath)
			: base(expression, type)
		{
		}

		/// <summary>
		/// Data formatting
		/// </summary>
		public Formatter[] Formatters { get; set; }
	}
}
