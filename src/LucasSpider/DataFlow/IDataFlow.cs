using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LucasSpider.DataFlow
{
    /// <summary>
    /// Data flow processor
    /// </summary>
    public interface IDataFlow : IDisposable
    {
        /// <summary>
        /// Initialization
        /// </summary>
        /// <returns></returns>
        Task InitializeAsync();

        /// <summary>
        /// Setup log
        /// </summary>
        /// <param name="logger"></param>
        void SetLogger(ILogger logger);

        /// <summary>
        /// Stream processing
        /// </summary>
        /// <param name="context">Processing context</param>
        /// <returns></returns>
        Task HandleAsync(DataFlowContext context);
    }
}
