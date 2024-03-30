namespace LucasSpider.AgentCenter
{
	public class AgentCenterOptions
	{
		/// <summary>
		/// Database connection string
		/// </summary>
		public string ConnectionString { get; set; }

		/// <summary>
		/// Database name
		/// </summary>
		public string Database { get; set; } = "LucasSpider";
	}
}
