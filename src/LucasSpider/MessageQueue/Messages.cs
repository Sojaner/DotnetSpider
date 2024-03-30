namespace LucasSpider.MessageQueue
{
	public static class Messages
	{
		public static class Agent
		{
			public class Exit : Message
			{
				public string AgentId { get; set; }
			}

			public class Heartbeat : Message
			{
				/// <summary>
				/// Logo
				/// </summary>
				public string AgentId { get; set; }

				/// <summary>
				/// Name
				/// </summary>
				public string AgentName { get; set; }

				/// <summary>
				/// Free memory
				/// </summary>
				public long AvailableMemory { get; set; }

				/// <summary>
				/// CPU load
				/// </summary>
				public int CpuLoad { get; set; }
			}

			public class Register : Message
			{
				/// <summary>
				/// Logo
				/// </summary>
				public string AgentId { get; set; }

				/// <summary>
				/// Name
				/// </summary>
				public string AgentName { get; set; }

				/// <summary>
				/// Number of CPU cores
				/// </summary>
				public int ProcessorCount { get; set; }

				/// <summary>
				/// Total memory
				/// </summary>
				public long Memory { get; set; }
			}
		}

		public static class Spider
		{
			public class Exit : Message
			{
				public string SpiderId { get; set; }
			}
		}

		public static class Statistics
		{
			public class AgentFailure : Message
			{
				public string AgentId { get; set; }
				public int ElapsedMilliseconds { get; set; }
			}

			public class AgentSuccess : Message
			{
				public string AgentId { get; set; }
				public int ElapsedMilliseconds { get; set; }
			}

			public class Exit : Message
			{
				public string SpiderId { get; set; }
			}

			public class Failure : Message
			{
				public string SpiderId { get; set; }
			}

			public class Print : Message
			{
				public string SpiderId { get; set; }
			}

			public class RegisterAgent
			{
				public string AgentId { get; set; }
				public string AgentName { get; set; }
			}

			public class Start : Message
			{
				public string SpiderId { get; set; }
				public string SpiderName { get; set; }
			}

			public class Success : Message
			{
				public string SpiderId { get; set; }
			}

			public class Total : Message
			{
				public string SpiderId { get; set; }
				public long Count { get; set; }
			}
		}
	}
}
