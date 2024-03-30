using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LucasSpider.Proxy
{
	public interface IProxyService
	{
		Task<Uri> GetAsync(int seconds);
		Uri Get();
		Task ReturnAsync(Uri proxy, HttpStatusCode statusCode);
		Task<int> AddAsync(IEnumerable<Uri> proxies);
	}
}
