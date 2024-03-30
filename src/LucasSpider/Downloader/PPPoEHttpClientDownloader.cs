using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LucasSpider.Http;
using LucasSpider.Proxy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ByteArrayContent = LucasSpider.Http.ByteArrayContent;
using Http_ByteArrayContent = LucasSpider.Http.ByteArrayContent;

namespace LucasSpider.Downloader
{
	public class PPPoEHttpClientDownloader : HttpClientDownloader
	{
		private readonly PPPoEService _pppoeService;

		public PPPoEHttpClientDownloader(IHttpClientFactory httpClientFactory,
			ILogger<PPPoEHttpClientDownloader> logger,
			IProxyService proxyService,
			PPPoEService pppoeService,
			IOptions<DownloaderOptions> options) : base(httpClientFactory, proxyService, logger, options)
		{
			_pppoeService = pppoeService;
			if (!_pppoeService.IsActive)
			{
				throw new SpiderException("PPoE configuration is incorrect");
			}
		}

		public override string Name => Downloaders.PPPoEHttpClient;

		protected override async Task<Response> HandleAsync(Request request, HttpResponseMessage responseMessage)
		{
			Response response = null;

			// Todo: Consider the issue of switching dialing during concurrent processes
			var text = await responseMessage.Content.ReadAsStringAsync();
			var validResult = await _pppoeService.DetectAsync(request, text);
			if (!string.IsNullOrWhiteSpace(validResult))
			{
				Logger.LogError(
					$"{request.RequestUri} download failed, because content contains {validResult}");
				response = new Response
				{
					RequestHash = request.Hash,
					StatusCode = HttpStatusCode.BadGateway,
					Content = new Http_ByteArrayContent(
						Encoding.UTF8.GetBytes($"Redial agent because content contains {validResult}"))
				};
			}

			return response;
		}
	}
}
