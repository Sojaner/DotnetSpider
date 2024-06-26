using System;
using System.Threading;
using System.Threading.Tasks;
using LucasSpider.DataFlow.Parser;
using LucasSpider.DataFlow.Storage;
using LucasSpider.Downloader;
using LucasSpider.Http;
using LucasSpider.Scheduler;
using LucasSpider.Scheduler.Component;
using LucasSpider.Selector;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace LucasSpider.Sample.samples
{
	public class JsonEntitySpider : Spider
	{
		public static async Task RunAsync()
		{
			var builder = Builder.CreateDefaultBuilder<JsonEntitySpider>();
			builder.UseSerilog();
			builder.UseDownloader<HttpClientDownloader>();
			builder.UseQueueDistinctBfsScheduler<HashSetDuplicateRemover>();
			await builder.Build().RunAsync();
		}

		public JsonEntitySpider(IOptions<SpiderOptions> options, DependenceServices services, ILogger<Spider> logger) :
			base(options, services, logger)
		{
		}

		protected override async Task InitializeAsync(CancellationToken stoppingToken)
		{
			AddDataFlow(new DataParser<MyEntity>());
			AddDataFlow(GetDefaultStorage());
			await AddRequestsAsync(
				new Request("file://samples/test.json") {Downloader = Downloaders.File});
		}

		[Schema("json", "data")]
		[EntitySelector(Expression = "$.[*]", Type = SelectorType.JsonPath)]
		class MyEntity : EntityBase<MyEntity>
		{
			[ValueSelector(Expression = "$.link", Type = SelectorType.JsonPath)]
			public string Link { get; set; }

			[ValueSelector(Expression = "$.video", Type = SelectorType.JsonPath)]
			public int Video { get; set; }

			[ValueSelector(Expression = "$.audio", Type = SelectorType.JsonPath)]
			public int Audio { get; set; }

			[ValueSelector(Expression = "DATETIME", Type = SelectorType.Environment)]
			public DateTime CreationTime { get; set; }
		}
	}
}
