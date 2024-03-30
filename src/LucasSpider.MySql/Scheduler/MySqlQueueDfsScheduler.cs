using LucasSpider.Infrastructure;
using Microsoft.Extensions.Options;

namespace LucasSpider.MySql.Scheduler
{
	public class MySqlQueueDfsScheduler : MySqlQueueScheduler
	{
		public MySqlQueueDfsScheduler(IRequestHasher requestHasher, IOptions<MySqlSchedulerOptions> options) : base(
			requestHasher, options)
		{
		}

		protected override string DequeueSql =>
			"SELECT id, hash, request FROM {0}_queue ORDER BY id DESC LIMIT {1} FOR UPDATE;";
	}
}
