using System.IO;
using System.Threading.Tasks;
using LucasSpider.DataFlow.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace LucasSpider.DataFlow
{
	/// <summary>
	/// JSON file saves parsing results (all parsing results)
	/// Save path: [current program running directory]/files/[task identification]/[request.hash].json
	/// </summary>
	public class JsonFileStorage : FileStorageBase
	{
		public static IDataFlow CreateFromOptions(IConfiguration configuration)
		{
			return new JsonFileStorage();
		}

		public override async Task HandleAsync(DataFlowContext context)
		{
			if (IsNullOrEmpty(context))
			{
				Logger.LogWarning("Data flow context does not contain parsing results");
				return;
			}

			var file = Path.Combine(GetDataFolder(context.Request.Owner),
				$"{context.Request.Hash}.json");
			using var writer = OpenWrite(file);
			var items = context.GetData();
			await writer.WriteLineAsync(JsonConvert.SerializeObject(items));
		}
	}
}
