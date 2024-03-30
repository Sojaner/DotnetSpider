using System.Threading.Tasks;
using LucasSpider.Infrastructure;
using Microsoft.Extensions.Logging;

namespace LucasSpider.DataFlow
{
	/// <summary>
	/// Data stream processor base class
	/// </summary>
	public abstract class DataFlowBase : IDataFlow
	{
		protected ILogger Logger { get; private set; }

		/// <summary>
		/// Initialization
		/// </summary>
		/// <returns></returns>
		public abstract Task InitializeAsync();

		public void SetLogger(ILogger logger)
		{
			logger.NotNull(nameof(logger));
			Logger = logger;
		}

		/// <summary>
		/// Stream processing
		/// </summary>
		/// <param name="context">Processing context</param>
		/// <returns></returns>
		public abstract Task HandleAsync(DataFlowContext context);

		/// <summary>
		/// Is it empty
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		protected virtual bool IsNullOrEmpty(DataFlowContext context)
		{
			return context.IsEmpty;
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public virtual void Dispose()
		{
		}
	}
}
