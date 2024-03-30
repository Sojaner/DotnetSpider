using System;
using System.Reflection;
using LucasSpider.MessageQueue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace LucasSpider.RabbitMQ
{
	public static class ServiceCollectionExtensions
	{
		public static Builder UseRabbitMQ(this Builder builder,
			Action<RabbitMQOptions> configureDelegate = null)
		{
			builder.ConfigureServices(services =>
			{
				var fields = typeof(HostBuilder).GetField("_appConfiguration",
					BindingFlags.NonPublic | BindingFlags.Instance);
				if (fields != null)
				{
					var configuration = (IConfiguration)fields.GetValue(builder);
					services.Configure<RabbitMQOptions>(configuration.GetSection("RabbitMQ"));
				}

				if (configureDelegate != null)
				{
					services.Configure(configureDelegate);
				}

				services.TryAddSingleton<IMessageQueue, RabbitMQMessageQueue>();
			});
			return builder;
		}

		public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<RabbitMQOptions>(configuration.GetSection("RabbitMQ"));
			services.TryAddSingleton<IMessageQueue, RabbitMQMessageQueue>();
			return services;
		}
	}
}
