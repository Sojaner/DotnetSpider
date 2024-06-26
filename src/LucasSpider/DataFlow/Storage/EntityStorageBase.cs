using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LucasSpider.DataFlow.Storage
{
	/// <summary>
	/// Physical memory
	/// </summary>
	public abstract class EntityStorageBase : DataFlowBase
	{
		private readonly Type _baseType = typeof(IEntity);

		/// <summary>
		///
		/// </summary>
		/// <param name="context">Data flow context</param>
		/// <param name="entities">Data parsing results (data type, List (data object))</param>
		/// <returns></returns>
		protected abstract Task HandleAsync(DataFlowContext context, IDictionary<Type, ICollection<dynamic>> entities);

		public override async Task HandleAsync(DataFlowContext context)
		{
			if (IsNullOrEmpty(context))
			{
				Logger.LogWarning("Data flow context does not contain entity resolution results");
				return;
			}

			var data = context.GetData();
			var result = new Dictionary<Type, ICollection<dynamic>>();
			foreach (var kv in data)
			{
				var type = kv.Key as Type;
				if (type == null || !_baseType.IsAssignableFrom(type))
				{
					continue;
				}

				if (kv.Value is IEnumerable list)
				{
					foreach (var obj in list)
					{
						AddResult(result, type, obj);
					}
				}
				else
				{
					AddResult(result, type, kv.Value);
				}
			}

			await HandleAsync(context, result);
		}

		private void AddResult(IDictionary<Type, ICollection<dynamic>> dict, Type type, dynamic obj)
		{
			if (!_baseType.IsInstanceOfType(obj))
			{
				return;
			}

			if (!dict.ContainsKey(type))
			{
				dict.Add(type, new List<dynamic>());
			}

			dict[type].Add(obj);
		}
	}
}
