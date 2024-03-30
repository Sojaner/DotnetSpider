namespace LucasSpider.Proxy
{
	public class ProxyOptions
	{
		/// <summary>
		/// Link to test whether the proxy is normal
		/// </summary>
		public string ProxyTestUrl { get; set; } = "http://www.bing.com";

		/// <summary>
		/// Agent Provisioning API
		/// Generally, proxy providers will provide API requests to return a list of available proxies.
		/// </summary>
		public string ProxySupplierUrl { get; set; }
	}
}
