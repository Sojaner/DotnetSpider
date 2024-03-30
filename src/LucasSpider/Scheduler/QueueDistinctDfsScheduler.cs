using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LucasSpider.Http;
using LucasSpider.Infrastructure;
using LucasSpider.Scheduler.Component;

namespace LucasSpider.Scheduler
{
	/// <summary>
	/// Memory-based depth-first scheduling (deduplicated URLs)
	/// </summary>
	public class QueueDistinctDfsScheduler : SchedulerBase
	{
		private readonly List<Request> _requests =
			new();

		public QueueDistinctDfsScheduler(IDuplicateRemover duplicateRemover, IRequestHasher requestHasher) :
			base(
				duplicateRemover, requestHasher)
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
			var dequeueCount = count;
			int start;
			if (_requests.Count < count)
			{
				dequeueCount = _requests.Count;
				start = 0;
			}
			else
			{
				start = _requests.Count - dequeueCount;
			}

			var requests = new List<Request>();
			for (var i = _requests.Count; i > start; --i)
			{
				requests.Add(_requests[i - 1]);
			}

			if (dequeueCount > 0)
			{
				_requests.RemoveRange(start, dequeueCount);
			}

			return Task.FromResult(requests.Select(x => x.Clone()));
		}
	}
}
