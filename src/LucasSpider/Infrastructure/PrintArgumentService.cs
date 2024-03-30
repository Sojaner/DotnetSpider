using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LucasSpider.Infrastructure
{
	public class PrintArgumentService : IHostedService
	{
		private readonly SpiderOptions _options;
		private readonly ILogger<PrintArgumentService> _logger;

		private static readonly string _logo = @"

 _                          _____       _     _
| |                        / ____|     (_)   | |
| |    _   _  ___ __ _ ___| (___  _ __  _  __| | ___ _ __
| |   | | | |/ __/ _` / __|\___ \| '_ \| |/ _` |/ _ \ '__|
| |___| |_| | (_| (_| \__ \____) | |_) | | (_| |  __/ |
|______\__,_|\___\__,_|___/_____/| .__/|_|\__,_|\___|_|     version: {0}
                                 | |
                                 |_|

";

		public PrintArgumentService(IOptions<SpiderOptions> options, ILogger<PrintArgumentService> logger)
		{
			_logger = logger;
			_options = options.Value;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			var properties = typeof(SpiderOptions).GetProperties();
			var version = GetType().Assembly.GetName().Version;
			var versionDescription = version.MinorRevision == 0 ? version.ToString() : $"{version}-beta";
			_logger.LogInformation(string.Format(_logo, versionDescription));
			foreach (var property in properties)
			{
				_logger.LogInformation($"{property.Name}: {property.GetValue(_options)}");
			}

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
