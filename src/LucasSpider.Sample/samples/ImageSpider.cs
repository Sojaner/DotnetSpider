using System.Threading;
using System.Threading.Tasks;
using LucasSpider.DataFlow;
using LucasSpider.Downloader;
using LucasSpider.Http;
using LucasSpider.Infrastructure;
using LucasSpider.Scheduler;
using LucasSpider.Scheduler.Component;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace LucasSpider.Sample.samples
{
	public class ImageSpider : Spider
	{
		public static async Task RunAsync()
		{
			var builder = Builder.CreateDefaultBuilder<ImageSpider>();
			builder.UseSerilog();
			builder.UseDownloader<HttpClientDownloader>();
			builder.UseQueueDistinctBfsScheduler<HashSetDuplicateRemover>();
			await builder.Build().RunAsync();
		}

		public ImageSpider(IOptions<SpiderOptions> options,
			DependenceServices services,
			ILogger<Spider> logger) : base(options, services, logger)
		{
		}

		protected override SpiderId GenerateSpiderId()
		{
			return new(ObjectId.CreateId().ToString(), "Github images");
		}

		protected override async Task InitializeAsync(CancellationToken stoppingToken = default)
		{
			await AddRequestsAsync(
				new Request("https://www.cnblogs.com/images/logo.svg?v=R9M0WmLAIPVydmdzE2keuvnjl-bPR7_35oHqtiBzGsM")
				{
					Headers = { { "test", "a" } }
				});
			AddDataFlow(new ImageStorage());
		}
	}
}
