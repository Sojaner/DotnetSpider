using System.Threading.Tasks;
using LucasSpider.Extensions;
using LucasSpider.MessageQueue;
using IMessageQueue = LucasSpider.MessageQueue.IMessageQueue;
using MessageQueue_IMessageQueue = LucasSpider.MessageQueue.IMessageQueue;

namespace LucasSpider.Statistics
{
	public class StatisticsClient : IStatisticsClient
	{
		private readonly MessageQueue_IMessageQueue _messageQueue;

		public StatisticsClient(MessageQueue_IMessageQueue messageQueue)
		{
			_messageQueue = messageQueue;
		}

		public async Task IncreaseTotalAsync(string id, long count)
		{
			await _messageQueue.PublishAsBytesAsync(Topics.Statistics,
				new Messages.Statistics.Total {SpiderId = id, Count = count});
		}

		public async Task IncreaseSuccessAsync(string id)
		{
			await _messageQueue.PublishAsBytesAsync(Topics.Statistics,
				new Messages.Statistics.Success {SpiderId = id});
		}

		public async Task IncreaseFailureAsync(string id)
		{
			await _messageQueue.PublishAsBytesAsync(Topics.Statistics,
				new Messages.Statistics.Failure {SpiderId = id});
		}

		public async Task StartAsync(string id, string name)
		{
			await _messageQueue.PublishAsBytesAsync(Topics.Statistics,
				new Messages.Statistics.Start {SpiderId = id, SpiderName = name});
		}

		public async Task ExitAsync(string id)
		{
			await _messageQueue.PublishAsBytesAsync(Topics.Statistics,
				new Messages.Statistics.Exit {SpiderId = id});
		}

		public async Task RegisterAgentAsync(string agentId, string agentName)
		{
			await _messageQueue.PublishAsBytesAsync(Topics.Statistics,
				new Messages.Statistics.RegisterAgent {AgentId = agentId, AgentName = agentName});
		}

		public async Task IncreaseAgentSuccessAsync(string agentId, int elapsedMilliseconds)
		{
			await _messageQueue.PublishAsBytesAsync(Topics.Statistics,
				new Messages.Statistics.AgentSuccess {AgentId = agentId, ElapsedMilliseconds = elapsedMilliseconds});
		}

		public async Task IncreaseAgentFailureAsync(string agentId, int elapsedMilliseconds)
		{
			await _messageQueue.PublishAsBytesAsync(Topics.Statistics,
				new Messages.Statistics.AgentFailure
				{
					AgentId = agentId,
					ElapsedMilliseconds = elapsedMilliseconds
				});
		}

		public async Task PrintAsync(string id)
		{
			await _messageQueue.PublishAsBytesAsync(Topics.Statistics,
				new Messages.Statistics.Print
				{
					SpiderId = id
				});
		}
	}
}
