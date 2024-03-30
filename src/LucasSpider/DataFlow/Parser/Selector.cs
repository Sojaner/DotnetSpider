using LucasSpider.Selector;
using Newtonsoft.Json;

namespace LucasSpider.DataFlow.Parser
{
	/// <summary>
	/// Selector properties
	/// </summary>
	public class Selector : System.Attribute
	{
#if !NET451
		/// <summary>
		/// Avoid being serialized
		/// </summary>
		[JsonIgnore]
		public override object TypeId => base.TypeId;
#endif

		/// <summary>
		/// Construction method
		/// </summary>
		public Selector()
		{
		}

		/// <summary>
		/// Construction method
		/// </summary>
		/// <param name="expression">Expression</param>
		/// <param name="type">Selector type</param>
		/// <param name="arguments">Parameters</param>
		public Selector(string expression, SelectorType type = SelectorType.XPath, string arguments = null)
		{
			Type = type;
			Expression = expression;
			Arguments = arguments;
		}

		/// <summary>
		/// Selector type
		/// </summary>
		public SelectorType Type { get; set; } = SelectorType.XPath;

		/// <summary>
		/// Expression
		/// </summary>
		public string Expression { get; set; }

		/// <summary>
		/// Parameter
		/// </summary>
		public string Arguments { get; set; }
	}
}
