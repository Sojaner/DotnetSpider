using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LucasSpider.DataFlow.Storage;
using Newtonsoft.Json;

namespace LucasSpider.DataFlow.Parser
{
	/// <summary>
	/// Model for parsing
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Model<T> where T : EntityBase<T>, new()
	{
		/// <summary>
		/// Entity type name
		/// </summary>
		public string TypeName { get; }

		/// <summary>
		/// Data model selector
		/// </summary>
		public Selector Selector { get; }

		/// <summary>
		/// Take the first Take entities from the final parsed result
		/// </summary>
		public int Take { get; }

		/// <summary>
		/// Set the direction of Take, the default is to take from the head
		/// </summary>
		public bool TakeByDescending { get; }

		/// <summary>
		/// Database column information defined by the crawler entity
		/// </summary>
		public HashSet<ValueSelector> ValueSelectors { get; }

		/// <summary>
		/// Target link selector
		/// </summary>
		public HashSet<FollowRequestSelector> FollowRequestSelectors { get; }

		/// <summary>
		/// Selectors for shared values
		/// </summary>
		public HashSet<GlobalValueSelector> GlobalValueSelectors { get; }

		/// <summary>
		/// Construction method
		/// </summary>
		public Model()
		{
			var type = typeof(T);
			TypeName = type.FullName;
			var entitySelector =
				type.GetCustomAttributes(typeof(EntitySelector), true).FirstOrDefault() as EntitySelector;
			var take = 0;
			var takeByDescending = false;
			Selector selector = null;
			if (entitySelector != null)
			{
				take = entitySelector.Take;
				takeByDescending = entitySelector.TakeByDescending;
				selector = new Selector {Expression = entitySelector.Expression, Type = entitySelector.Type};
			}

			var followRequestSelectors = type.GetCustomAttributes(typeof(FollowRequestSelector), true).Select(x => (FollowRequestSelector) x)
				.ToList();
			var sharedValueSelectors = type.GetCustomAttributes(typeof(GlobalValueSelector), true)
				.Select(x => (GlobalValueSelector) x).ToList();

			var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

			var valueSelectors = new HashSet<ValueSelector>();
			foreach (var property in properties)
			{
				var valueSelector =
					property.GetCustomAttributes(typeof(ValueSelector), true).FirstOrDefault() as ValueSelector;

				if (valueSelector == null)
				{
					continue;
				}

				valueSelector.Formatters = property.GetCustomAttributes(typeof(Formatter), true)
					.Select(p => (Formatter) p).ToArray();
				valueSelector.PropertyInfo = property;
				valueSelector.NotNull = property.GetCustomAttributes(typeof(Required), false).Any();
				valueSelectors.Add(valueSelector);
			}

			Selector = selector;
			ValueSelectors = valueSelectors;
			FollowRequestSelectors = new HashSet<FollowRequestSelector>(followRequestSelectors);
			GlobalValueSelectors = new HashSet<GlobalValueSelector>(sharedValueSelectors);
			foreach (var valueSelector in GlobalValueSelectors)
			{
				if (string.IsNullOrWhiteSpace(valueSelector.Name))
				{
					throw new SpiderException("Name of global value selector should not be null");
				}
			}

			Take = take;
			TakeByDescending = takeByDescending;
		}
	}
}
