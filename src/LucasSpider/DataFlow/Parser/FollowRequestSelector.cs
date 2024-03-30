using System;
using LucasSpider.Selector;
using Newtonsoft.Json;

namespace LucasSpider.DataFlow.Parser
{
	/// <summary>
	/// Target link selector definition
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class FollowRequestSelector : Attribute
	{
		/// <summary>
		/// Query type
		/// </summary>
		public SelectorType SelectorType { get; set; } = SelectorType.XPath;

		/// <summary>
		/// Query expression
		/// </summary>
		public string[] Expressions { get; set; }

#if !NET451
		/// <summary>
		/// Avoid being serialized
		/// </summary>
		[JsonIgnore]
		public override object TypeId => base.TypeId;
#endif

		/// <summary>
		/// Regular expression to match target link
		/// </summary>
		public string[] Patterns { get; set; } = new string[0];
	}
}
