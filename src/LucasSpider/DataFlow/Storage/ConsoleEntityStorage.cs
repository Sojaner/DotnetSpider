using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LucasSpider.DataFlow.Storage;
using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace LucasSpider.DataFlow
{
	/// <summary>
	/// Console print (entity) parsing results
	/// </summary>
	public class ConsoleEntityStorage : EntityStorageBase
	{
		public static IDataFlow CreateFromOptions(IConfiguration configuration)
		{
			return new ConsoleEntityStorage();
		}

		public override Task InitializeAsync()
		{
			return Task.CompletedTask;
		}

		protected override Task HandleAsync(DataFlowContext context, IDictionary<Type, ICollection<dynamic>> entities)
		{
			foreach (var kv in entities)
			{
				Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(kv.Value));
			}

			return Task.CompletedTask;
		}
	}
}
