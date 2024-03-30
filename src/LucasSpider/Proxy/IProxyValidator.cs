using System;
using System.Threading.Tasks;

namespace LucasSpider.Proxy
{
    public interface IProxyValidator
    {
        Task<bool> IsAvailable(Uri proxy);
    }
}
