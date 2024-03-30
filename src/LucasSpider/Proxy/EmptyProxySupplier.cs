using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LucasSpider.Proxy
{
	public class EmptyProxySupplier : IProxySupplier
	{
		public Task<IEnumerable<Uri>> GetProxiesAsync()
		{
			return Task.FromResult(Enumerable.Empty<Uri>());
		}
	}
}
