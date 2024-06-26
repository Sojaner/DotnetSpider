using LucasSpider.Statistics.Store;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LucasSpider.Statistics
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddStatistics<T>(this IServiceCollection services)
			where T : class, IStatisticsStore
		{
			services.TryAddSingleton<IStatisticsStore, T>();
			services.AddHostedService<StatisticsService>();
			return services;
		}
	}
}
