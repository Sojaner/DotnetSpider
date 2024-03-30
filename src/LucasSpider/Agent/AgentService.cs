using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LucasSpider.Extensions;
using LucasSpider.Downloader;
using LucasSpider.Http;
using LucasSpider.Infrastructure;
using LucasSpider.MessageQueue;
using LucasSpider.Statistics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IMessageQueue = LucasSpider.MessageQueue.IMessageQueue;
using MessageQueue_IMessageQueue = LucasSpider.MessageQueue.IMessageQueue;

namespace LucasSpider.Agent
{
	public class AgentService : BackgroundService
	{
		private readonly ILogger<AgentService> _logger;
		private readonly MessageQueue_IMessageQueue _messageQueue;
		private readonly List<AsyncMessageConsumer<byte[]>> _consumers;
		private readonly IHostApplicationLifetime _applicationLifetime;
		private readonly IDownloader _downloader;
		private readonly AgentOptions _options;
		private readonly IStatisticsClient _statisticsClient;

		public AgentService(
			MessageQueue_IMessageQueue messageQueue,
			IOptions<AgentOptions> options,
			IHostApplicationLifetime applicationLifetime,
			IDownloader downloader, HostBuilderContext hostBuilderContext, ILogger<AgentService> logger,
			IStatisticsClient statisticsClient)
		{
			_options = options.Value;
			_logger = logger;
			_statisticsClient = statisticsClient;
			_messageQueue = messageQueue;
			_applicationLifetime = applicationLifetime;
			_downloader = downloader;
			_consumers = new List<AsyncMessageConsumer<byte[]>>();

			hostBuilderContext.Properties[Const.DefaultDownloader] = downloader.Name;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogDebug(
				_messageQueue.IsDistributed
					? $"Agent {_options.AgentId}, {_options.AgentName} is starting"
					: "Agent is starting");

			await _statisticsClient.RegisterAgentAsync(_options.AgentId, _options.AgentName);

			// Distributed only requires registration
			if (_messageQueue.IsDistributed)
			{
				await _messageQueue.PublishAsBytesAsync(Topics.AgentCenter,
					new Messages.Agent.Register
					{
						AgentId = _options.AgentId,
						AgentName = _options.AgentName,
						Memory = MachineInfo.Current.Memory,
						ProcessorCount = Environment.ProcessorCount
					});
			}

			// Downloaders of the same type are registered in the same topic for load balancing.
			await RegisterAgentAsync(_downloader.Name, stoppingToken);

			if (_messageQueue.IsDistributed)
			{
				// Register agent_{id} for fixed node download
				await RegisterAgentAsync(string.Format(Topics.Spider, _options.AgentId), stoppingToken);

				// Distributed only needs to send heartbeats
				Task.Factory.StartNew(async () =>
				{
					while (!stoppingToken.IsCancellationRequested)
					{
						await HeartbeatAsync();
						await Task.Delay(5000, stoppingToken);
					}
				}, stoppingToken).ConfigureAwait(true).GetAwaiter();
			}

			_logger.LogInformation(_messageQueue.IsDistributed
				? $"Agent {_options.AgentId}, {_options.AgentName} started"
				: "Agent started");
		}

		private async Task RegisterAgentAsync(string topic, CancellationToken stoppingToken)
		{
			var consumer = new AsyncMessageConsumer<byte[]>(topic);
			consumer.Received += HandleMessageAsync;
			await _messageQueue.ConsumeAsync(consumer, stoppingToken);
			_consumers.Add(consumer);
		}

		private async Task HandleMessageAsync(byte[] bytes)
		{
			object message;
			try
			{
				message = await bytes.DeserializeAsync();
				if (message == null)
				{
					return;
				}
			}
			catch (Exception e)
			{
				_logger.LogError($"Deserializing message failed: {e}");
				return;
			}

			switch (message)
			{
				case Messages.Agent.Exit exit:
					{
						if (exit.AgentId == _options.AgentId)
						{
							_applicationLifetime.StopApplication();
						}

						break;
					}
				case Request request:
					Task.Factory.StartNew(async () =>
					{
						var response = await _downloader.DownloadAsync(request);
						if (response == null)
						{
							return;
						}

						response.Agent = _options.AgentId;

						var topic = string.Format(Topics.Spider, request.Owner);
						await _messageQueue.PublishAsBytesAsync(topic, response);

						_logger.LogInformation(
							_messageQueue.IsDistributed
								? $"Agent {_options.AgentName} download {request.RequestUri}, {request.Hash} for {request.Owner} completed"
								: $"{request.Owner} download {request.RequestUri}, {request.Hash} completed");
					}).ConfigureAwait(false).GetAwaiter();
					break;
				default:
					{
						var msg = JsonSerializer.Serialize(message);
						_logger.LogWarning($"Message not supported: {msg}");
						break;
					}
			}
		}

		private async Task HeartbeatAsync()
		{
			_logger.LogInformation($"Heartbeat {_options.AgentId}, {_options.AgentName}");

			await _messageQueue.PublishAsBytesAsync(Topics.AgentCenter,
				new Messages.Agent.Heartbeat
				{
					AgentId = _options.AgentId,
					AgentName = _options.AgentName,
					AvailableMemory = MachineInfo.Current.AvailableMemory,
					CpuLoad = 0
				});
		}

		public override async Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation(
				_messageQueue.IsDistributed ? $"Agent {_options.AgentId} is stopping" : "Agent is stopping");

			foreach (var consumer in _consumers)
			{
				consumer.Close();
			}

			await base.StopAsync(cancellationToken);

			_logger.LogInformation(_messageQueue.IsDistributed ? $"Agent {_options.AgentId} stopped" : "Agent stopped");
		}
	}
}
