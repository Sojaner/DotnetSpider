using System;
using System.Threading.Tasks;
using LucasSpider.Http;

namespace LucasSpider.Scheduler.Component
{
	public interface IDuplicateRemover : IDisposable
	{
		/// <summary>
		/// Check whether the request is duplicate.
		/// </summary>
		/// <param name="request">Request</param>
		/// <returns>Whether the request is duplicate.</returns>
		Task<bool> IsDuplicateAsync(Request request);

		/// <summary>
		/// Initialization
		/// </summary>
		/// <param name="spiderId"></param>
		Task InitializeAsync(string spiderId);

		/// <summary>
		/// Get total
		/// </summary>
		Task<long> GetTotalAsync();

		/// <summary>
		/// Reset duplicate check.
		/// </summary>
		Task ResetDuplicateCheckAsync();
	}
}
