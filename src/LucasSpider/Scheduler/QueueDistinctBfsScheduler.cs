using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LucasSpider.Http;
using LucasSpider.Infrastructure;
using LucasSpider.Scheduler.Component;

namespace LucasSpider.Scheduler
{
	/// <summary>
	/// Memory-based breadth-first scheduling (deduplicated URLs)
	/// </summary>
	public class QueueDistinctBfsScheduler : SchedulerBase
	{
		private readonly List<Request> _requests =
			new();

		public QueueDistinctBfsScheduler(IDuplicateRemover duplicateRemover, IRequestHasher requestHasher)
			: base(duplicateRemover, requestHasher)
		{
		}

		public override void Dispose()
		{
			_requests.Clear();
			base.Dispose();
		}

		/// <summary>
		/// If the request is not repeated, add it to the queue
		/// </summary>
		/// <param name="request">Request</param>
		protected override Task PushWhenNoDuplicate(Request request)
		{
			if (request == null)
			{
				return Task.CompletedTask;
			}
			_requests.Add(request);
			return Task.CompletedTask;
		}

		/// <summary>
		/// Remove the specified number of requests for the specified crawler from the queue
		/// </summary>
		/// <param name="count">Number of outputs</param>
		/// <returns>Request</returns>
		protected override Task<IEnumerable<Request>> ImplDequeueAsync(int count = 1)
		{
			var requests = _requests.Take(count).ToArray();

			if (requests.Length > 0)
			{
				_requests.RemoveRange(0, requests.Length);
			}

			return Task.FromResult(requests.Select(x => x.Clone()));
		}
	}
}
