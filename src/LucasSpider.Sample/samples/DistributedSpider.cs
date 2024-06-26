using System.Threading.Tasks;
using LucasSpider.RabbitMQ;
using LucasSpider.Scheduler;
using LucasSpider.Scheduler.Component;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace LucasSpider.Sample.samples
{
	public class DistributedSpider
	{
		public static async Task RunAsync()
		{
			var builder = Builder.CreateBuilder<TestSpider2>(options =>
			{
				options.Speed = 2;
			});
			builder.UseSerilog();
			builder.UseRabbitMQ();
			builder.UseQueueDistinctBfsScheduler<HashSetDuplicateRemover>();
			await builder.Build().RunAsync();
		}
	}
}
