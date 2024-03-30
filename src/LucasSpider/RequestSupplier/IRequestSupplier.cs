using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LucasSpider.Http;

namespace LucasSpider.RequestSupplier
{
    /// <summary>
    /// Request supply interface
    /// </summary>
    public interface IRequestSupplier
    {
        /// <summary>
        /// Run request provisioning
        /// </summary>
        Task<IEnumerable<Request>> GetAllListAsync(CancellationToken cancellationToken);
    }
}
