using System.Threading;
using System.Threading.Tasks;
using LucasSpider.DataFlow;
using LucasSpider.DataFlow.Parser;
using LucasSpider.Http;
using LucasSpider.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LucasSpider.Sample.samples
{
	public class GithubSpider : Spider
	{
		public GithubSpider(IOptions<SpiderOptions> options, DependenceServices services, ILogger<Spider> logger) :
			base(options, services, logger)
		{
		}

		protected override async Task InitializeAsync(CancellationToken stoppingToken)
		{
			// Add custom parsing
			AddDataFlow(new Parser());
			// Using console memory
			AddDataFlow(new ConsoleStorage());
			// Add collection request
			await AddRequestsAsync(new Request("https://github.com/zlzforever")
			{
				// Request timeout 10 seconds
				Timeout = 10000
			});
		}

		protected override SpiderId GenerateSpiderId()
		{
			return new(ObjectId.CreateId().ToString(), "Github");
		}

		class Parser : DataParser
		{
			public override Task InitializeAsync()
			{
				return Task.CompletedTask;
			}

			protected override Task ParseAsync(DataFlowContext context)
			{
				var selectable = context.Selectable;
				// Analytical data
				var author = selectable.XPath("//span[@class='p-name vcard-fullname d-block overflow-hidden']")
					?.Value;
				var name = selectable.XPath("//span[@class='p-nickname vcard-username d-block']")
					?.Value;
				context.AddData("author", author);
				context.AddData("username", name);
				return Task.CompletedTask;
			}
		}
	}
}
