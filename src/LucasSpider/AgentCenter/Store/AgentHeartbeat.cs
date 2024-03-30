using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LucasSpider.Infrastructure;

namespace LucasSpider.AgentCenter.Store
{
	[Table("agent_heartbeat")]
	public class AgentHeartbeat
	{
		/// <summary>
		/// Node ID
		/// </summary>
		// ReSharper disable once UnusedAutoPropertyAccessor.Local
		public long Id { get; private set; }

		/// <summary>
		/// Logo
		/// </summary>
		[StringLength(36)]
		[Column("agent_id")]
		public string AgentId { get; private set; }

		/// <summary>
		/// Name
		/// </summary>
		[StringLength(255)]
		[Column("agent_name")]
		public string AgentName { get; private set; }

		/// <summary>
		/// Free memory
		/// </summary>
		[Column("available_memory")]
		public long AvailableMemory { get; private set; }

		/// <summary>
		/// CPU load
		/// </summary>
		[Column("cpu_load")]
		public int CpuLoad { get; private set; }

		/// <summary>
		/// Reporting time
		/// </summary>
		[Column("creation_time")]
		public DateTimeOffset CreationTime { get; private set; }

		public AgentHeartbeat(string agentId, string agentName, long freeMemory, int cpuLoad)
		{
			agentId.NotNullOrWhiteSpace(nameof(agentId));

			AgentId = agentId;
			AgentName = agentName;
			AvailableMemory = freeMemory;
			CpuLoad = cpuLoad;
			CreationTime = DateTimeOffset.Now;
		}

		public override string ToString()
		{
			return $"Id {Id}, CreationTime {CreationTime}";
		}
	}
}
