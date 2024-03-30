using System.IO;
using System.Threading.Tasks;
using LucasSpider.DataFlow.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace LucasSpider.DataFlow
{
	/// <summary>
	/// File saves analysis results (all analysis results)
	/// Save path: [current program running directory]/files/[task identification]/[request.hash].data
	/// </summary>
	public class FileStorage : FileStorageBase
	{
		public static IDataFlow CreateFromOptions(IConfiguration configuration)
		{
			return new FileStorage();
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
			var items = context
				.GetData();
			await writer.WriteLineAsync(System.Text.Json.JsonSerializer.Serialize(new
			{
				uri = context.Request.RequestUri.ToString(), data = items
			}));
		}
	}
}
