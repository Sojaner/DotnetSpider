using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LucasSpider.Proxy
{
    public interface IProxySupplier
    {
        Task<IEnumerable<Uri>> GetProxiesAsync();
    }
}
