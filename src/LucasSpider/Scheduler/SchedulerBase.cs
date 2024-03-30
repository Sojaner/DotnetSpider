using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LucasSpider.Http;
using LucasSpider.Infrastructure;
using LucasSpider.Scheduler.Component;

namespace LucasSpider.Scheduler
{
	public abstract class SchedulerBase : IScheduler
	{
		private SpinLock _spinLock;
		private readonly IRequestHasher _requestHasher;

		protected readonly IDuplicateRemover DuplicateRemover;

		protected SchedulerBase(IDuplicateRemover duplicateRemover, IRequestHasher requestHasher)
		{
			DuplicateRemover = duplicateRemover;
			_requestHasher = requestHasher;
		}

		/// <summary>
		/// Reset deduplicator
		/// </summary>
		public virtual async Task ResetDuplicateCheckAsync()
		{
			await DuplicateRemover.ResetDuplicateCheckAsync();
		}

		public virtual Task SuccessAsync(Request request)
		{
			return Task.CompletedTask;
		}

		public virtual Task FailAsync(Request request)
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// If the request is not repeated, add it to the queue
		/// </summary>
		/// <param name="request">Request</param>
		protected abstract Task PushWhenNoDuplicate(Request request);

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public virtual void Dispose()
		{
			DuplicateRemover.Dispose();
		}

		/// <summary>
		/// The total number of requests in the queue
		/// </summary>
		public async Task<long> GetTotalAsync()
		{
			return await DuplicateRemover.GetTotalAsync();
		}

		/// <summary>
		/// Remove the specified number of requests for the specified crawler from the queue
		/// </summary>
		/// <param name="count">Number of outputs</param>
		/// <returns>Request</returns>
		protected abstract Task<IEnumerable<Request>> ImplDequeueAsync(int count = 1);

		public virtual async Task InitializeAsync(string spiderId)
		{
			await DuplicateRemover.InitializeAsync(spiderId);
		}

		public async Task<IEnumerable<Request>> DequeueAsync(int count = 1)
		{
			var locker = false;

			try
			{
				//Apply for a lock
				_spinLock.Enter(ref locker);

				return await ImplDequeueAsync(count);
			}
			finally
			{
				//When the work is completed or an exception occurs, check whether the current thread holds the lock. If we have the lock, release it
				//To avoid deadlock situations
				if (locker)
				{
					_spinLock.Exit();
				}
			}
		}

		/// <summary>
		/// Request to join the queue
		/// </summary>
		/// <param name="requests">Requests</param>
		/// <returns>Number of queued entries</returns>
		public async Task<int> EnqueueAsync(IEnumerable<Request> requests)
		{
			var count = 0;
			foreach (var request in requests)
			{
				_requestHasher.ComputeHash(request);
				if (await DuplicateRemover.IsDuplicateAsync(request))
				{
					continue;
				}

				await PushWhenNoDuplicate(request);
				count++;
			}

			return count;
		}
	}
}
