namespace LucasSpider.Downloader
{
	public class DownloaderOptions
	{
		/// <summary>
		/// Should the downloader track redirects
		/// </summary>
		/// <remarks>Default is <b>true</b></remarks>
		public bool TrackRedirects { get; set; } = true;

		/// <summary>
		/// Maximum number of allowed redirects
		/// </summary>
		/// <remarks>Default is <b>5</b></remarks>
		public int MaximumAllowedRedirects { get; set; } = 5;

		/// <summary>
		/// The playwright browser to use
		/// </summary>
		/// <remarks>Default is <b>Chromium</b></remarks>
		public PlaywrightBrowserName BrowserName { get; set; } = PlaywrightBrowserName.Chromium;
	}
}
