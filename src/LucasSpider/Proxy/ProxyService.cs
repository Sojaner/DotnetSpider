using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using LucasSpider.Extensions;
using HWT;
using Microsoft.Extensions.Logging;

namespace LucasSpider.Proxy
{
	public class ProxyService : IProxyService
	{
		private class ProxyEntry
		{
			public Uri Uri { get; private set; }

			/// <summary>
			/// The number of failed data downloads using this proxy
			/// </summary>
			public int FailedNum { get; set; }

			public ProxyEntry(Uri uri)
			{
				Uri = uri;
			}
		}

		private readonly ConcurrentQueue<ProxyEntry> _queue;
		private readonly ConcurrentDictionary<Uri, ProxyEntry> _dict;
		private readonly IProxyValidator _proxyValidator;
		private readonly ILogger<ProxyService> _logger;

		private readonly HashedWheelTimer _timer = new(TimeSpan.FromSeconds(1)
			, 100000);

		private readonly int _ignoreCount;
		private readonly int _reDetectCount;

		public ProxyService(IProxyValidator proxyValidator, ILogger<ProxyService> logger)
		{
			_proxyValidator = proxyValidator;
			_logger = logger;
			_queue = new ConcurrentQueue<ProxyEntry>();
			_dict = new ConcurrentDictionary<Uri, ProxyEntry>();
			_ignoreCount = 6;
			_reDetectCount = _ignoreCount / 2;
		}

		public async Task ReturnAsync(Uri proxy, HttpStatusCode statusCode)
		{
			if (_dict.TryGetValue(proxy, out var p))
			{
				// If success is returned, the number of failures will be directly set to 0.
				if (statusCode.IsSuccessStatusCode())
				{
					p.FailedNum = 0;
				}
				else
				{
					p.FailedNum += 1;
				}

				// If the number of failures is greater than ignoreCount, the agent will be deleted from the cache and no longer used.
				if (p.FailedNum > _ignoreCount)
				{
					_dict.TryRemove(p.Uri, out _);
					return;
				}

				// If the number of failures is a multiple of reDetectCount, try to retest whether the proxy is normal. If the test is unsuccessful, delete the proxy from the cache and no longer use it.
				if ((p.FailedNum != 0 && p.FailedNum % _reDetectCount == 0) &&
				    !await _proxyValidator.IsAvailable(p.Uri))
				{
					_dict.TryRemove(p.Uri, out _);
					return;
				}

				_queue.Enqueue(p);
			}
		}

		public async Task<int> AddAsync(IEnumerable<Uri> proxies)
		{
			var cnt = 0;
			foreach (var proxy in proxies)
			{
				if (await _proxyValidator.IsAvailable(proxy) && _dict.TryAdd(proxy, new ProxyEntry(proxy)))
				{
					_logger.LogInformation($"proxy {proxy} is available");
					_queue.Enqueue(_dict[proxy]);
					cnt++;
				}
			}

			return cnt;
		}

		public async Task<Uri> GetAsync(int seconds)
		{
			var waitCount = seconds * 10;
			for (var i = 0; i < waitCount; ++i)
			{
				var proxy = Get();
				if (proxy != null)
				{
					return proxy;
				}

				await Task.Delay(100);
			}

			return null;
		}

		public Uri Get()
		{
			if (_queue.TryDequeue(out var proxy))
			{
				_timer.NewTimeout(new ReturnProxyTask(this, proxy.Uri), TimeSpan.FromSeconds(30));
				return proxy.Uri;
			}
			else
			{
				return null;
			}
		}

		private class ReturnProxyTask : ITimerTask
		{
			private readonly Uri _proxy;
			private readonly IProxyService _pool;

			public ReturnProxyTask(IProxyService pool, Uri proxy)
			{
				_pool = pool;
				_proxy = proxy;
			}

			public async Task RunAsync(ITimeout timeout)
			{
				await _pool.ReturnAsync(_proxy, HttpStatusCode.OK);
			}
		}
	}
}
