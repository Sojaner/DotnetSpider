using System;

namespace LucasSpider.DataFlow.Parser
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class GlobalValueSelector : ValueSelector
	{
		/// <summary>
		/// The name of the parsed value
		/// </summary>
		public string Name { get; set; }
	}
}
