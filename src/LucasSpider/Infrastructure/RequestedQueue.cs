using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
#if NETSTANDARD2_0
using System.Threading;
#endif
using System.Threading.Tasks;
using HWT;
using LucasSpider.Http;
using Microsoft.Extensions.Logging;

namespace LucasSpider.Infrastructure
{
	public class RequestedQueue : IDisposable
	{
		private readonly ConcurrentDictionary<string, Request> _dict;
		private readonly HashedWheelTimer _timer;
		private readonly List<TimeoutTask> _tasks;
		private ConcurrentBag<Request> _queue;

		public RequestedQueue()
		{
			_dict = new ConcurrentDictionary<string, Request>();
			_queue = new ConcurrentBag<Request>();
			_timer = new HashedWheelTimer(TimeSpan.FromSeconds(1)
				, 100000);
			_tasks = new List<TimeoutTask>();
		}

		public int Count => _dict.Count;

		public bool Enqueue(Request request)
		{
			if (request.Timeout < 2000)
			{
				throw new SpiderException("Timeout should not less than 2000 milliseconds");
			}

			if (!_dict.TryAdd(request.Hash, request))
			{
				return false;
			}

			var task = new TimeoutTask(this, request.Hash);
			_tasks.Add(task);
			_timer.NewTimeout(task,
				TimeSpan.FromMilliseconds(request.Timeout));
			return true;
		}


		public Request Dequeue(string hash)
		{
			return _dict.TryRemove(hash, out var request) ? request : null;
		}

		public Request[] GetAllTimeoutList()
		{
			var data = _queue.ToArray();
#if NETSTANDARD2_0
			Interlocked.Exchange(ref _queue, new ConcurrentBag<Request>());
#else
			_queue.Clear();
#endif
			return data;
		}

		private void Timeout(string hash)
		{
			if (_dict.TryRemove(hash, out var request))
			{
				_queue.Add(request);
			}
		}

		private class TimeoutTask : ITimerTask, IDisposable
		{
			private readonly string _hash;
			private readonly RequestedQueue _requestedQueue;

			public TimeoutTask(RequestedQueue requestedQueue, string hash)
			{
				_hash = hash;
				_requestedQueue = requestedQueue;
			}

			public Task RunAsync(ITimeout timeout)
			{
				_requestedQueue.Timeout(_hash);
				return Task.CompletedTask;
			}

			public void Dispose()
			{
				_requestedQueue?.Dispose();
			}
		}

		public void Dispose()
		{
			_dict.Clear();
#if NETSTANDARD2_0
			Interlocked.Exchange(ref _queue, new ConcurrentBag<Request>());
#else
			_queue.Clear();
#endif
			_timer.Stop();
			_timer.Dispose();
			_tasks.ForEach(task => ObjectUtilities.DisposeSafely(task));
		}
	}
}
