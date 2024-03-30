using System;
using System.Threading.Tasks;

namespace LucasSpider.Proxy
{
	public class FakeProxyValidator : IProxyValidator
	{
		public Task<bool> IsAvailable(Uri proxy)
		{
			return Task.FromResult(true);
		}
	}
}
