using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LucasSpider.Proxy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LucasSpider.Downloader
{
	public class FakeHttpClientDownloader : HttpClientDownloader
	{
		public FakeHttpClientDownloader(IHttpClientFactory httpClientFactory,
			IProxyService proxyService,
			ILogger<HttpClientDownloader> logger,
			IOptions<DownloaderOptions> options) : base(httpClientFactory, proxyService, logger, options)
		{
		}

		protected override Task<HttpResponseMessage> SendAsync(HttpClient httpClient,
			HttpRequestMessage httpRequestMessage)
		{
			return Task.FromResult(new HttpResponseMessage
			{
				Content = new StringContent("<html></html>", Encoding.UTF8),
				RequestMessage = httpRequestMessage,
				StatusCode = HttpStatusCode.OK,
				Version = HttpVersion.Version11
			});
		}

		public override string Name => UseProxy ? Downloaders.FakeProxyHttpClient : Downloaders.FakeHttpClient;
	}
}
