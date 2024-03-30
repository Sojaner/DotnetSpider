using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
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
	public class BaseUsageSpider : Spider
	{
		public static async Task RunAsync()
		{
			var builder = Builder.CreateDefaultBuilder<BaseUsageSpider>(x =>
			{
				x.Batch = 1;
				x.Speed = 1;
				x.Depth = 1;
				x.DefaultTimeout = 15000;
				x.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36";
				x.RetriedTimes = 0;
			});
			builder.UseSerilog();
			builder.UseDownloader<PlaywrightDownloader>(options => options.BrowserName = PlaywrightBrowserName.WebKit);
			builder.UseQueueDistinctBfsScheduler<HashSetDuplicateRemover>();
			await builder.Build().RunAsync();
		}

		private const string Domain = "www.ledigajobb.nu";

		class MyDataParser : DataParser
		{
			private readonly ConcurrentDictionary<string, string> _dictionary = new();

			protected override Task ParseAsync(DataFlowContext context)
			{
				var links = context.Selectable.Links().ToList();

				var uri = context.Request.RequestUri.AbsoluteUri;

				foreach (var link in links)
				{
					_dictionary.TryAdd(link, uri);
				}

				if (context.Selectable is HtmlSelectable)
				{
				}

				return Task.CompletedTask;
			}

			public override Task HandleAsync(DataFlowContext context)
			{
				return context.Response.IsSuccessStatusCode && (context.Response.Content.Headers.ContentType?.StartsWith(MediaTypeNames.Text.Html) ?? false)
					? base.HandleAsync(context)
					: Task.CompletedTask;
			}

			public override Task InitializeAsync()
			{
				//AddRequiredValidator(request => request.RequestUri.Scheme is "http" or "https" && !MimeTypePredictor.IsKnownExtension(request.RequestUri) && request.RequestUri.DnsSafeHost == Domain);
				//AddFollowRequestQuerier(Selectors.XPath("//a")); // //a[not(@rel='nofollow')]
				return Task.CompletedTask;
			}
		}

		public BaseUsageSpider(IOptions<SpiderOptions> options, DependenceServices services,
			ILogger<Spider> logger) : base(
			options, services, logger)
		{
			OnRequestError += async (r, re) =>
			{
				await Task.CompletedTask;
			};

			OnRequestTimeout += _ =>
			{
			};
		}

		protected override async Task InitializeAsync(CancellationToken stoppingToken = default)
		{
			//await AddRequestsAsync("https://httpstat.us/200?sleep=10000");
			await AddRequestsAsync($"http://{Domain}");
			AddDataFlow(new MyDataParser());
			AddDataFlow(new ConsoleStorage());
		}

		protected override SpiderId GenerateSpiderId()
		{
			return new(ObjectId.CreateId().ToString(), "Blog Garden");
		}
	}

    public static class MimeTypePredictor
    {
        private static HashSet<string> _skippedExtensions;

        static MimeTypePredictor()
        {
            InitializeMapping();
        }

        private static void InitializeMapping()
        {
	        _skippedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
	        {
		        ".txt",
		        ".css",
		        ".js",
		        ".json",
		        ".xml",
		        ".jpg",
		        ".jpeg",
		        ".png",
		        ".gif",
		        ".pdf",
		        ".woff",
		        ".woff2",
		        ".ttf",
		        ".eot",
		        ".otf",
		        ".svg",
		        ".mp4",
		        ".webm",
		        ".webp",
		        ".ogg",
		        ".mp3",
		        ".doc",
		        ".docx",
		        ".xls",
		        ".xlsx",
		        ".ppt",
		        ".pptx",
		        ".zip",
		        ".rar",
		        ".tar",
		        ".gz",
		        ".7z",
		        ".exe",
		        ".dll",
		        ".bin",
		        ".iso",
		        ".csv",
		        ".sql",
		        ".mdb",
		        ".mpg",
		        ".mpeg",
		        ".avi",
		        ".mov",
		        ".wav",
		        ".aac",
		        ".flac",
		        ".bmp",
		        ".ico",
		        ".tif",
		        ".tiff",
		        ".psd",
		        ".ai"
	        };
        }

        public static bool IsKnownExtension(Uri uri)
        {
            var extension = Path.GetExtension(uri.AbsolutePath);

            return _skippedExtensions.Contains(extension.ToLower());
        }
    }
}
