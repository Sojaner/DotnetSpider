using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using LucasSpider.DataFlow.Storage;
using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace LucasSpider.DataFlow
{
	/// <summary>
	/// JSON file saves parsing (entity) results
	/// Save path: [current program running directory]/files/[task identification]/[request.hash].data
	/// </summary>
	public class JsonEntityFileStorage : EntityFileStorageBase
	{
		private readonly ConcurrentDictionary<string, StreamWriter> _streamWriters =
			new();

		/// <summary>
		/// Returns memory based on configuration
		/// </summary>
		/// <param name="configuration">Deployment</param>
		/// <returns></returns>
		public static JsonEntityFileStorage CreateFromOptions(IConfiguration configuration)
		{
			return new();
		}

		public override Task InitializeAsync()
		{
			return Task.CompletedTask;
		}

		protected override async Task HandleAsync(DataFlowContext context, TableMetadata tableMetadata,
			IEnumerable entities)
		{
			var streamWriter = _streamWriters.GetOrAdd(tableMetadata.TypeName,
				_ => OpenWrite(context, tableMetadata, "json"));
			await streamWriter.WriteLineAsync(System.Text.Json.JsonSerializer.Serialize(entities));
		}

		public override void Dispose()
		{
			foreach (var streamWriter in _streamWriters)
			{
				streamWriter.Value.Dispose();
			}

			base.Dispose();
		}
	}
}
