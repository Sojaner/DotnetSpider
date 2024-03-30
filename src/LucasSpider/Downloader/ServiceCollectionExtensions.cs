using System;
using System.Linq;
using System.Net.Http;
using LucasSpider.Agent;
using LucasSpider.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;

namespace LucasSpider.Downloader
{
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Only local crawlers can configure downloader. The downloader registration for distributed crawlers is in the downloader agent.
		/// </summary>
		/// <param name="builder">Host builder</param>
		/// <typeparam name="TDownloader">The downloader to use</typeparam>
		/// <returns>Host builder</returns>
		public static Builder UseDownloader<TDownloader>(this Builder builder)
			where TDownloader : class, IDownloader
		{
			return UseDownloader<TDownloader>(builder, options =>
			{
				options.MaximumAllowedRedirects = 5;
				options.TrackRedirects = true;
			});
		}

		/// <summary>
		/// Only local crawlers can configure downloader. The downloader registration for distributed crawlers is in the downloader agent.
		/// </summary>
		/// <param name="builder">Host builder</param>
		/// <param name="configureOptions">Configure downloader options</param>
		/// <typeparam name="TDownloader">The downloader to use</typeparam>
		/// <returns>Host builder</returns>
		public static Builder UseDownloader<TDownloader>(this Builder builder, Action<DownloaderOptions> configureOptions)
			where TDownloader : class, IDownloader
		{
			builder.ConfigureServices(collection =>
			{
				collection.Configure(configureOptions);

				var genericType = typeof(TDownloader);
				var constructors = genericType.GetConstructors();
				if (constructors.Select(constructor => constructor.GetParameters()).Any(parameters => parameters.Any(p => typeof(IHttpClientFactory).IsAssignableFrom(p.ParameterType))))
				{
					collection.AddTransient<HttpMessageHandlerBuilder, DefaultHttpMessageHandlerBuilder>();
				}
				if (constructors.Select(constructor => constructor.GetParameters()).Any(parameters => parameters.Any(p => typeof(IBrowser).IsAssignableFrom(p.ParameterType))))
				{
					collection.AddSingleton(provider =>
					{
						return provider.GetService<IOptions<DownloaderOptions>>().Value.BrowserName switch
						{
							PlaywrightBrowserName.Chromium => Playwright.CreateAsync().Result.Chromium.LaunchAsync().Result,
							PlaywrightBrowserName.Firefox => Playwright.CreateAsync().Result.Firefox.LaunchAsync().Result,
							PlaywrightBrowserName.WebKit => Playwright.CreateAsync().Result.Webkit.LaunchAsync().Result,
							_ => throw new NotSupportedException("Not supported browser")
						};
					});
				}

				collection.AddAgent<TDownloader>(opts =>
				{
					opts.AgentId = ObjectId.CreateId().ToString();
					opts.AgentName = opts.AgentId;
				});
			});

			return builder;
		}
	}
}
