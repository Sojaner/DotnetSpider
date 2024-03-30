namespace LucasSpider.Portal.ViewObject
{
	public class AgentViewObject
	{
		/// <summary>
		/// Logo
		/// </summary>
		public virtual string Id { get; set; }

		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Number of CPU cores
		/// </summary>
		public int ProcessorCount { get; set; }

		/// <summary>
		/// Total memory
		/// </summary>
		public int TotalMemory { get; set; }

		/// <summary>
		/// Last updated
		/// </summary>
		public string LastModificationTime { get; set; }

		/// <summary>
		/// Has it been marked for deletion?
		/// </summary>
		public bool IsDeleted { get; set; }

		/// <summary>
		/// Creation time
		/// </summary>
		public string CreationTime { get; set; }

		public bool Online { get; set; }
	}
}
