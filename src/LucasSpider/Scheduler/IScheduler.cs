using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LucasSpider.Http;

namespace LucasSpider.Scheduler
{
	/// <summary>
	/// Scheduler interface
	/// </summary>
	public interface IScheduler : IDisposable
	{
		/// <summary>
		/// Initialization
		/// </summary>
		/// <param name="spiderId"></param>
		Task InitializeAsync(string spiderId);

		/// <summary>
		/// Remove the specified number of requests for the specified crawler from the queue
		/// </summary>
		/// <param name="count">Number of outputs</param>
		/// <returns>Request</returns>
		Task<IEnumerable<Request>> DequeueAsync(int count = 1);

		/// <summary>
		/// Request to join the queue
		/// </summary>
		/// <param name="requests">Requests</param>
		/// <returns>Number of queued entries</returns>
		Task<int> EnqueueAsync(IEnumerable<Request> requests);

		/// <summary>
		/// The total number of requests in the queue
		/// </summary>
		Task<long> GetTotalAsync();

		/// <summary>
		/// Reset
		/// </summary>
		/// <returns></returns>
		Task ResetDuplicateCheckAsync();

		/// <summary>
		/// Tag request successful
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		Task SuccessAsync(Request request);

		/// <summary>
		/// Tag request failed
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		Task FailAsync(Request request);
	}
}
