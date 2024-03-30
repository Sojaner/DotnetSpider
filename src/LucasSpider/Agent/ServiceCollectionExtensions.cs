using System;
using LucasSpider.Downloader;
using LucasSpider.Proxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LucasSpider.Agent
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddAgent<TDownloader>(this IServiceCollection services,
			Action<AgentOptions> configure = null)
			where TDownloader : class, IDownloader
		{
			services.AddHttpClient();

			if (configure != null)
			{
				services.Configure(configure);
			}

			services.TryAddSingleton<IProxyService, EmptyProxyService>();
			services.AddSingleton<IDownloader, TDownloader>();
			services.AddHostedService<AgentService>();
			return services;
		}
	}
}
