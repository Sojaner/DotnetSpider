using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace LucasSpider.DataFlow
{
	/// <summary>
	/// The console prints the parsing results (all parsing results)
	/// </summary>
	public class ConsoleStorage : DataFlowBase
	{
		public static IDataFlow CreateFromOptions(IConfiguration configuration)
		{
			return new ConsoleStorage();
		}

		public override Task InitializeAsync()
		{
			return Task.CompletedTask;
		}

		public override Task HandleAsync(DataFlowContext context)
		{
			if (IsNullOrEmpty(context))
			{
				Logger.LogWarning("Data flow context does not contain parsing results");
				return Task.CompletedTask;
			}

			var data = context.GetData();

			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine(
				$"{Environment.NewLine}DATA: {System.Text.Json.JsonSerializer.Serialize(data)}");

			return Task.CompletedTask;
		}
	}
}
