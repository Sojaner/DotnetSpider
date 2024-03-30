using System.Collections.Generic;
using System.Threading.Tasks;

namespace LucasSpider.AgentCenter.Store
{
    public interface IAgentStore
    {
        /// <summary>
        /// Create database and tables
        /// </summary>
        /// <returns></returns>
        Task EnsureDatabaseAndTableCreatedAsync();

        /// <summary>
        /// Query all registered downloader agents
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<AgentInfo>> GetAllListAsync();

        /// <summary>
        /// Add downloader agent
        /// </summary>
        /// <param name="agent">Downloader agent</param>
        /// <returns></returns>
        Task RegisterAsync(AgentInfo agent);

        /// <summary>
        /// Save heartbeat of downloader agent
        /// </summary>
        /// <param name="heartbeat">Downloader agent heartbeat</param>
        /// <returns></returns>
        Task HeartbeatAsync(AgentHeartbeat heartbeat);
    }
}
