using System.Threading.Tasks;
using LucasSpider.Infrastructure;

namespace LucasSpider.Statistics.Store
{
	public interface IStatisticsStore
	{
		/// <summary>
		/// Create database and tables
		/// </summary>
		/// <returns></returns>
		Task EnsureDatabaseAndTableCreatedAsync();

		/// <summary>
		/// Add 1 to the total number of requests
		/// </summary>
		/// <param name="id">Crawler ID</param>
		/// <param name="count"></param>
		/// <returns></returns>
		Task IncreaseTotalAsync(string id, long count);

		/// <summary>
		/// Add 1 to the number of successes
		/// </summary>
		/// <param name="id">Crawler ID</param>
		/// <returns></returns>
		Task IncreaseSuccessAsync(string id);

		/// <summary>
		/// Add 1 to the number of failures
		/// </summary>
		/// <param name="id">Crawler ID</param>
		/// <returns></returns>
		Task IncreaseFailureAsync(string id);

		/// <summary>
		/// Crawler starts
		/// </summary>
		/// <param name="id">Crawler ID</param>
		/// <param name="name">Crawler name</param>
		/// <returns></returns>
		Task StartAsync(string id, string name);

		/// <summary>
		/// Crawler exits
		/// </summary>
		/// <param name="id">Crawler ID</param>
		/// <returns></returns>
		Task ExitAsync(string id);

		/// <summary>
		/// Register node
		/// </summary>
		/// <param name="agentId"></param>
		/// <param name="agentName"></param>
		/// <returns></returns>
		Task RegisterAgentAsync(string agentId, string agentName);

		/// <summary>
		/// Add 1 to the number of successful downloads
		/// </summary>
		/// <param name="agentId">Download agent ID</param>
		/// <param name="elapsedMilliseconds">Total download time</param>
		/// <returns></returns>
		Task IncreaseAgentSuccessAsync(string agentId, int elapsedMilliseconds);

		/// <summary>
		/// Add 1 to the number of failed downloads
		/// </summary>
		/// <param name="agentId">Download agent ID</param>
		/// <param name="elapsedMilliseconds">Total download time</param>
		/// <returns></returns>
		Task IncreaseAgentFailureAsync(string agentId, int elapsedMilliseconds);

		/// <summary>
		/// Query the statistical information of the download agent by pagination
		/// </summary>
		/// <param name="agentId"></param>
		/// <param name="page"></param>
		/// <param name="limit"></param>
		/// <returns></returns>
		Task<PagedResult<AgentStatistics>> PagedQueryAgentStatisticsAsync(string agentId, int page, int limit);

		/// <summary>
		/// Query the statistics of a specified download agent
		/// </summary>
		/// <param name="agentId">Download agent ID</param>
		/// <returns></returns>
		Task<AgentStatistics> GetAgentStatisticsAsync(string agentId);

		/// <summary>
		/// Query the statistics of a specified crawler
		/// </summary>
		/// <param name="id">Crawler ID</param>
		/// <returns></returns>
		Task<SpiderStatistics> GetSpiderStatisticsAsync(string id);

		/// <summary>
		/// Query crawler statistics by page
		/// </summary>
		/// <param name="keyword"></param>
		/// <param name="page"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		Task<PagedResult<SpiderStatistics>> PagedQuerySpiderStatisticsAsync(string keyword, int page, int size);
	}
}
