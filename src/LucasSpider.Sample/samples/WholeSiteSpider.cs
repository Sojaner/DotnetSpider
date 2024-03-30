using System.Threading;
using System.Threading.Tasks;
using LucasSpider.DataFlow;
using LucasSpider.DataFlow.Parser;
using LucasSpider.Downloader;
using LucasSpider.Infrastructure;
using LucasSpider.Scheduler;
using LucasSpider.Scheduler.Component;
using LucasSpider.Selector;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace LucasSpider.Sample.samples
{
	public class WholeSiteSpider : Spider
	{
		public static async Task RunAsync()
		{
			var builder = Builder.CreateDefaultBuilder<WholeSiteSpider>(options =>
			{
				options.Depth = 1000;
			});
			builder.UseDownloader<HttpClientDownloader>();
			builder.UseSerilog();
			builder.UseQueueDistinctBfsScheduler<HashSetDuplicateRemover>();
			await builder.Build().RunAsync();
		}

		public WholeSiteSpider(IOptions<SpiderOptions> options,
			DependenceServices services,
			ILogger<Spider> logger) : base(
			options, services, logger)
		{
		}

		protected override async Task InitializeAsync(CancellationToken stoppingToken)
		{
			AddDataFlow(new MyDataParser());
			AddDataFlow(new ConsoleStorage()); // Console prints collection results
			await AddRequestsAsync("http://www.cnblogs.com/"); // Set starting link
		}

		protected override SpiderId GenerateSpiderId()
		{
			return new(ObjectId.CreateId().ToString(), "Blog Park full site collection");
		}

		class MyDataParser : DataParser
		{
			public override Task InitializeAsync()
			{
				AddRequiredValidator("cnblogs\\.com");
				AddFollowRequestQuerier(Selectors.XPath("."));
				return Task.CompletedTask;
			}

			protected override Task ParseAsync(DataFlowContext context)
			{
				context.AddData("URL", context.Request.RequestUri);
				context.AddData("Title", context.Selectable.XPath(".//title")?.Value);
				return Task.CompletedTask;
			}
		}
	}
}
