namespace LucasSpider.Portal.ViewObject
{
	public class AgentHeartbeatViewObject
	{
		/// <summary>
		/// Node ID
		/// </summary>
		public int Id { get; set; }

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
		public int AvailableMemory { get; set; }

		public int CpuLoad { get; set; }

		/// <summary>
		/// Reporting time
		/// </summary>
		public string CreationTime { get; set; }
	}
}
