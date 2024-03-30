using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Bert.RateLimiters;
using LucasSpider.Extensions;
using LucasSpider.DataFlow;
using LucasSpider.DataFlow.Parser;
using LucasSpider.DataFlow.Storage;
using LucasSpider.Downloader;
using LucasSpider.Http;
using LucasSpider.Infrastructure;
using LucasSpider.MessageQueue;
using LucasSpider.RequestSupplier;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

[assembly: InternalsVisibleTo("LucasSpider.Tests")]

namespace LucasSpider
{
	public abstract class Spider :
		BackgroundService
	{
		private readonly IList<IDataFlow> _dataFlows;
		private readonly IList<IRequestSupplier> _requestSuppliers;
		private readonly RequestedQueue _requestedQueue;
		private AsyncMessageConsumer<byte[]> _consumer;
		private readonly DependenceServices _services;
		private readonly string _defaultDownloader;
		private readonly IList<DataParser> _dataParsers;

		/// <summary>
		/// Request Timeout event
		/// </summary>
		protected event Action<Request[]> OnRequestTimeout;

		/// <summary>
		/// Request error event
		/// </summary>
		protected event Action<Request, Response> OnRequestError;

		/// <summary>
		/// No new request events in scheduler
		/// </summary>
		protected event Action OnSchedulerEmpty;

		protected SpiderOptions Options { get; }

		/// <summary>
		/// Spider ID
		/// </summary>
		protected SpiderId SpiderId { get; private set; }

		/// <summary>
		/// Log interface
		/// </summary>
		protected ILogger Logger { get; }

		/// <summary>
		/// Is it a distributed crawler?
		/// </summary>
		protected bool IsDistributed => _services.MessageQueue.IsDistributed;

		protected Spider(IOptions<SpiderOptions> options,
			DependenceServices services,
			ILogger<Spider> logger
		)
		{
			Logger = logger;
			Options = options.Value;

			if (Options.Speed > 500)
			{
				throw new SpiderException("Speed should not be larger than 500");
			}

			if (Options.DefaultTimeout < 2000)
			{
				throw new SpiderException("The DefaultTimeout cannot be shorter than 2000 milliseconds");
			}

			_services = services;
			_requestedQueue = new RequestedQueue();
			_requestSuppliers = new List<IRequestSupplier>();
			_dataFlows = new List<IDataFlow>();
			_dataParsers = new List<DataParser>();

			_defaultDownloader = _services.HostBuilderContext.Properties.ContainsKey(Const.DefaultDownloader)
				? _services.HostBuilderContext.Properties[Const.DefaultDownloader]?.ToString()
				: Downloaders.HttpClient;
		}

		/// <summary>
		/// Initialize Spider data
		/// </summary>
		/// <param name="stoppingToken"></param>
		/// <returns></returns>
		protected abstract Task InitializeAsync(CancellationToken stoppingToken = default);

		/// <summary>
		/// Get the Spider ID and name
		/// </summary>
		/// <returns></returns>
		protected virtual SpiderId GenerateSpiderId()
		{
			var id = Environment.GetEnvironmentVariable("DOTNET_SPIDER_ID");
			id = string.IsNullOrWhiteSpace(id) ? ObjectId.CreateId().ToString() : id;
			var name = Environment.GetEnvironmentVariable("DOTNET_SPIDER_NAME");
			return new SpiderId(id, name);
		}

		protected IDataFlow GetDefaultStorage()
		{
			return StorageUtilities.CreateStorage(Options.StorageType, _services.Configuration);
		}

		public override async Task StopAsync(CancellationToken cancellationToken)
		{
			_consumer?.Close();
			_services.MessageQueue.CloseQueue(SpiderId.Id);

			await base.StopAsync(cancellationToken);

			Dispose();

			Logger.LogInformation($"{SpiderId} stopped");
		}

		/// <summary>
		/// Configure a request (dequeued from Scheduler)
		/// </summary>
		/// <param name="request"></param>
		protected virtual void ConfigureRequest(Request request)
		{
		}

		protected virtual Spider AddRequestSupplier(IRequestSupplier requestSupplier)
		{
			requestSupplier.NotNull(nameof(requestSupplier));
			_requestSuppliers.Add(requestSupplier);
			return this;
		}

		protected virtual Spider AddDataFlow(IDataFlow dataFlow)
		{
			dataFlow.NotNull(nameof(dataFlow));
			_dataFlows.Add(dataFlow);
			return this;
		}

		protected async Task<int> AddRequestsAsync(params string[] requests)
		{
			if (requests == null || requests.Length == 0)
			{
				return 0;
			}

			return await AddRequestsAsync(requests.Select(uri => new Request(uri) { Timeout = Options.DefaultTimeout, Headers = { UserAgent = Options.UserAgent }}));
		}

		protected async Task<int> AddRequestsAsync(params Request[] requests)
		{
			if (requests == null || requests.Length == 0)
			{
				return 0;
			}

			return await AddRequestsAsync(requests.AsEnumerable());
		}

		protected async Task<int> AddRequestsAsync(IEnumerable<Request> requests)
		{
			if (requests == null)
			{
				return 0;
			}

			var list = new List<Request>();

			foreach (var request in requests)
			{
				if (string.IsNullOrWhiteSpace(request.Downloader)
				    && !string.IsNullOrWhiteSpace(_defaultDownloader))
				{
					request.Downloader = _defaultDownloader;
				}

				if (request.Downloader.Contains("PPPoE") &&
				    string.IsNullOrWhiteSpace(request.PPPoERegex))
				{
					throw new ArgumentException(
						$"Request {request.RequestUri}, {request.Hash} set to use PPPoE but PPPoERegex is empty");
				}

				// 1. If the number of requests exceeds the limit, it will be skipped and a failure record will be added.
				// 2. The number of requests constructed by default is 0, and users are not allowed to change it, so data security can be guaranteed.
				if (request.RequestedTimes > Options.RetriedTimes)
				{
					await _services.StatisticsClient.IncreaseFailureAsync(SpiderId.Id);
					continue;
				}

				// 1. The default construction depth is 0, and users are not allowed to change it, so data security can be guaranteed.
				// 2. Skip when the depth exceeds the limit
				if (Options.Depth > 0 && request.Depth > Options.Depth)
				{
					continue;
				}

				request.Owner = SpiderId.Id;

				list.Add(request);
			}

			var count = await _services.Scheduler.EnqueueAsync(list);
			return count;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			SpiderId = GenerateSpiderId();
			Logger.LogInformation($"Initializing spider {SpiderId}, {SpiderId.Name}");
			await _services.StatisticsClient.StartAsync(SpiderId.Id, SpiderId.Name);
			await _services.Scheduler.InitializeAsync(SpiderId.Id);
			await InitializeAsync(stoppingToken);
			await InitializeDataFlowsAsync();
			await LoadRequestFromSuppliers(stoppingToken);
			await _services.StatisticsClient.IncreaseTotalAsync(SpiderId.Id, await _services.Scheduler.GetTotalAsync());
			await RegisterConsumerAsync(stoppingToken);
			await RunAsync(stoppingToken);
		}

		private async Task RegisterConsumerAsync(CancellationToken stoppingToken)
		{
			var topic = string.Format(Topics.Spider, SpiderId.Id);

			Logger.LogInformation($"{SpiderId} register topic {topic}");
			_consumer = new AsyncMessageConsumer<byte[]>(topic);
			_consumer.Received += async bytes =>
			{
				object message;
				try
				{
					message = await bytes.DeserializeAsync(stoppingToken);
					if (message == null)
					{
						return;
					}
				}
				catch (Exception e)
				{
					Logger.LogError($"Deserializing message failed: {e}");
					return;
				}

				switch (message)
				{
					case Messages.Spider.Exit exit:
						{
							Logger.LogInformation(
								$"{SpiderId} received exit message {System.Text.Json.JsonSerializer.Serialize(exit)}");
							if (exit.SpiderId == SpiderId.Id)
							{
								await ExitAsync();
							}

							break;
						}
					case Response response:
						{
							// 1. Remove the request from the request queue
							// 2. If it is a timeout request, it cannot be obtained through Dequeue, but will be obtained through _requestedQueue.GetAllTimeoutList()
							var request = _requestedQueue.Dequeue(response.RequestHash);

							if (request != null)
							{
								if (response.StatusCode.IsSuccessStatusCode())
								{
									request.Agent = response.Agent;

									if (IsDistributed)
									{
										Logger.LogInformation(
											$"{SpiderId} download {request.RequestUri}, {request.Hash} via {request.Agent} success");
									}

									// Whether the download is successful or not is determined by the crawler, not the Agent itself.
									await _services.StatisticsClient.IncreaseAgentSuccessAsync(response.Agent,
										response.Elapsed.Milliseconds);
									await HandleResponseAsync(request, response, bytes);
								}
								else
								{
									await _services.StatisticsClient.IncreaseAgentFailureAsync(response.Agent,
										response.Elapsed.Milliseconds);
									Logger.LogError(
										$"{SpiderId} download {request.RequestUri}, {request.Hash} status code: {response.StatusCode} failed: {response.ReasonPhrase}");

									request.RequestedTimes += 1;
									await AddRequestsAsync(request);

									OnRequestError?.Invoke(request, response);
								}
							}

							break;
						}
					default:
						Logger.LogError(
							$"{SpiderId} received error message {System.Text.Json.JsonSerializer.Serialize(message)}");
						break;
				}
			};

			await _services.MessageQueue.ConsumeAsync(_consumer, stoppingToken);
		}

		private async Task HandleResponseAsync(Request request, Response response, byte[] messageBytes)
		{
			DataFlowContext context = null;
			try
			{
				using var scope = _services.ServiceProvider.CreateScope();
				context = new DataFlowContext(scope.ServiceProvider, Options, request, response)
				{
					MessageBytes = messageBytes
				};

				foreach (var dataFlow in _dataFlows)
				{
					await dataFlow.HandleAsync(context);
				}

				var count = await AddRequestsAsync(context.FollowRequests);
				await _services.StatisticsClient.IncreaseTotalAsync(SpiderId.Id, count);

				// Add a successful request
				await _services.StatisticsClient.IncreaseSuccessAsync(SpiderId.Id);
				await _services.Scheduler.SuccessAsync(request);
			}
			// DataFlow can abort the crawler program by throwing ExitException
			catch (ExitException ee)
			{
				Logger.LogError($"Exit: {ee}");
				await ExitAsync();
			}
			catch (Exception e)
			{
				await _services.Scheduler.FailAsync(request);
				// if download correct content, parser or storage failed by network or something else
				// retry it until trigger retryTimes limitation
				await AddRequestsAsync(request);
				Logger.LogError($"{SpiderId} handle {System.Text.Json.JsonSerializer.Serialize(request)} failed: {e}");
			}
			finally
			{
				ObjectUtilities.DisposeSafely(context);
			}
		}

		/// <summary>
		/// If there is no data parser, it is considered that there is no need for a data parser, and it is passed directly to the memory and returns true.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		private bool IsValidRequest(Request request)
		{
			if (_dataParsers == null || _dataParsers.Count == 0)
			{
				return true;
			}

			return _dataParsers.Count > 0 && _dataParsers.Any(x => x.IsValidRequest(request));
		}

		private async Task RunAsync(CancellationToken stoppingToken)
		{
			try
			{
				var sleepTimeLimit = Options.EmptySleepTime * 1000;

				var speed = Options.Speed;
				var bucket = CreateBucket(speed);
				var sleepTime = 0;
				var batch = (int)Options.Batch;
				var start = DateTime.Now;
				var end = start;

				PrintStatistics(stoppingToken);

				while (!stoppingToken.IsCancellationRequested)
				{
					if (_requestedQueue.Count > Options.RequestedQueueCount)
					{
						sleepTime += 10;

						if (await WaitForContinueAsync(sleepTime, sleepTimeLimit, (end - start).TotalSeconds,
							    $"{SpiderId} has too many requests enqueued"))
						{
							continue;
						}
						else
						{
							break;
						}
					}

					if (await HandleTimeoutRequestAsync())
					{
						continue;
					}

					// If the batch size is changed, we should update the batch size
					if (batch != Options.Batch)
					{
						batch = (int)Options.Batch;
					}

					var requests = (await _services.Scheduler.DequeueAsync(batch)).ToArray();

					if (requests.Length > 0)
					{
						sleepTime = 0;

						foreach (var request in requests)
						{
							ConfigureRequest(request);

							// If there is no Parser that can handle this request, there is no need to download it.
							// https://github.com/dotnetcore/LucasSpider/issues/182
							if (!IsValidRequest(request))
							{
								continue;
							}

							while (bucket.ShouldThrottle(1, out var waitTimeMillis))
							{
								await Task.Delay(waitTimeMillis, default);
							}

							// If the speed is changed, the bucket needs to be recreated
							// Since the maximum speed is 500, the smallest division is 0.002, so the comparison is accurate to 0.001
							if (Math.Abs(speed - Options.Speed) > 0.001)
							{
								speed = Options.Speed;

								bucket = CreateBucket(speed);
							}

							if (!await PublishRequestMessagesAsync(request))
							{
								Logger.LogError("Exit by publish request message failed");
								break;
							}
						}

						end = DateTime.Now;
					}
					else
					{
						OnSchedulerEmpty?.Invoke();

						sleepTime += 10;

						if (!await WaitForContinueAsync(sleepTime, sleepTimeLimit, (end - start).TotalSeconds))
						{
							break;
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.LogError($"{SpiderId} exited with exception: {e}");
			}
			finally
			{
				await ExitAsync();
			}
		}

		private async Task<bool> HandleTimeoutRequestAsync()
		{
			var timeoutRequests = _requestedQueue.GetAllTimeoutList();
			if (timeoutRequests.Length <= 0)
			{
				return false;
			}

			foreach (var request in timeoutRequests)
			{
				request.RequestedTimes += 1;

				Logger.LogWarning(
					$"{SpiderId} request {request.RequestUri}, {request.Hash} timed out");
			}

			await AddRequestsAsync(timeoutRequests);

			OnRequestTimeout?.Invoke(timeoutRequests);

			return true;
		}

		private async Task<bool> WaitForContinueAsync(int sleepTime, int sleepTimeLimit, double totalSeconds,
			string waitMessage = null)
		{
			if (sleepTime > sleepTimeLimit)
			{
				Logger.LogInformation($"Exit: {(int)totalSeconds} seconds");
				return false;
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(waitMessage))
				{
					Logger.LogInformation(waitMessage);
				}

				await Task.Delay(10, default);
				return true;
			}
		}

		private void PrintStatistics(CancellationToken stoppingToken)
		{
			if (!IsDistributed)
			{
				Task.Factory.StartNew(async () =>
				{
					while (!stoppingToken.IsCancellationRequested)
					{
						await Task.Delay(5000, stoppingToken);
						await _services.StatisticsClient.PrintAsync(SpiderId.Id);
					}
				}, stoppingToken).ConfigureAwait(false).GetAwaiter();
			}
		}

		private async Task ExitAsync()
		{
			await _services.StatisticsClient.ExitAsync(SpiderId.Id);
			_services.ApplicationLifetime.StopApplication();
		}

		private static FixedTokenBucket CreateBucket(double speed)
		{
			if (speed >= 1)
			{
				var defaultTimeUnit = (int)(1000 / speed);
				return new FixedTokenBucket(1, 1, defaultTimeUnit);
			}
			else
			{
				var defaultTimeUnit = (int)((1 / speed) * 1000);
				return new FixedTokenBucket(1, 1, defaultTimeUnit);
			}
		}

		private async Task<bool> PublishRequestMessagesAsync(params Request[] requests)
		{
			if (requests.Length > 0)
			{
				foreach (var request in requests)
				{
					string topic;
					request.Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
					if (string.IsNullOrWhiteSpace(request.Agent))
					{
						topic = string.IsNullOrEmpty(request.Downloader)
							? Downloaders.HttpClient
							: request.Downloader;
					}
					else
					{
						switch (request.Policy)
						{
							// Non-initial requests use the old downloader if they are in chain mode
							case RequestPolicy.Chained:
								{
									topic = $"{request.Agent}";
									break;
								}
							case RequestPolicy.Random:
								{
									topic = string.IsNullOrEmpty(request.Downloader)
										? Downloaders.HttpClient
										: request.Downloader;
									break;
								}
							default:
								{
									throw new ApplicationException($"Not supported policy: {request.Policy}");
								}
						}
					}

					if (_requestedQueue.Enqueue(request))
					{
						await _services.MessageQueue.PublishAsBytesAsync(topic, request);
					}
					else
					{
						Logger.LogWarning(
							$"{SpiderId} enqueuing request: {request.RequestUri}, {request.Hash} failed");
					}
				}
			}

			return true;
		}

		protected async Task LoadRequestFromSuppliers(CancellationToken stoppingToken)
		{
			// Add request via provisioning interface
			foreach (var requestSupplier in _requestSuppliers)
			{
				foreach (var request in await requestSupplier.GetAllListAsync(stoppingToken))
				{
					await AddRequestsAsync(request);
				}

				Logger.LogInformation(
					$"{SpiderId} loaded request from {requestSupplier.GetType().Name} {_requestSuppliers.IndexOf(requestSupplier)}/{_requestSuppliers.Count}");
			}
		}

		private async Task InitializeDataFlowsAsync()
		{
			if (_dataFlows.Count == 0)
			{
				Logger.LogWarning($"{SpiderId} has no dataFlow");
			}
			else
			{
				var dataFlowInfo = string.Join(" -> ", _dataFlows.Select(x => x.GetType().Name));
				Logger.LogInformation($"{SpiderId} DataFlows: {dataFlowInfo}");
				foreach (var dataFlow in _dataFlows)
				{
					dataFlow.SetLogger(Logger);
					try
					{
						await dataFlow.InitializeAsync();
						if (dataFlow is DataParser dataParser)
						{
							_dataParsers.Add(dataParser);
						}
					}
					catch (Exception e)
					{
						Logger.LogError(
							$"{SpiderId} initializing dataFlow {dataFlow.GetType().Name} failed: {e}");
						_services.ApplicationLifetime.StopApplication();
					}
				}
			}
		}

		public override void Dispose()
		{
			ObjectUtilities.DisposeSafely(Logger, _requestedQueue);
			ObjectUtilities.DisposeSafely(Logger, _dataFlows);
			ObjectUtilities.DisposeSafely(Logger, _services);

			base.Dispose();

			GC.Collect();
		}
	}
}
