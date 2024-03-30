using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using LucasSpider.Http;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;

namespace LucasSpider.Downloader
{
	public class PlaywrightDownloader(IBrowser browser, IOptions<DownloaderOptions> options) : IDownloader
	{
		public async Task<Response> DownloadAsync(Request request)
		{
			try
			{
				await using var context = await browser.NewContextAsync(new BrowserNewContextOptions
				{
					UserAgent = request.Headers.UserAgent
				});
				context.SetDefaultNavigationTimeout(request.Timeout);

				var page = await context.NewPageAsync();
				var requests = new List<IRequest>();
				page.Request += onRequest;
				var stopwatch = Stopwatch.StartNew();
				await page.GotoAsync(request.RequestUri.AbsoluteUri);
				stopwatch.Stop();

				await page.WaitForLoadStateAsync();
				page.Request -= onRequest;
				var document = await (requests
					.SingleOrDefault(r => r.Frame == page.MainFrame && r.IsNavigationRequest &&
					                      r.RedirectedTo == null && r.ResourceType == "document")?
					.ResponseAsync() ?? Task.FromResult<IResponse>(null));

				if (document == null)
				{
					return Response.CreateFailedResponse(ResponseReasonPhraseConstants.NoResponse, request.Hash);
				}

				var redirects = (await GetRedirectsAsync(document)).ToList();
				var httpResponse = await ConvertIResponseToHttpResponse(document);

				var response = await httpResponse.ToResponseAsync();
				response.Elapsed = TimeSpan.FromMilliseconds(document.Request.Timing.ResponseEnd);
				response.RequestHash = request.Hash;
				response.Version = httpResponse.Version;
				response.Redirects = options.Value.TrackRedirects ? redirects : [];
				response.TimeToHeaders = TimeSpan.FromMilliseconds(document.Request.Timing.ResponseStart);
				response.TargetUrl = new Uri(document.Url);

				await context.CloseAsync();

				return response;
				void onRequest(object _, IRequest r) => requests.Add(r);
			}
			catch (Exception e)
			{
				return Response.CreateFailedResponse(e, request.Hash);
			}
		}

		private static async Task<IEnumerable<RedirectResponse>> GetRedirectsAsync(IResponse response)
		{
			var list = new List<RedirectResponse>();
			var parent = response.Request.RedirectedFrom;
			var responseStart = response.Request.Timing.ResponseStart; // This is a fallback for WebKit browser since it doesn't have response start time in the redirect chain

			while (parent != null)
			{
				var parentResponse = await parent.ResponseAsync();
				if (parentResponse != null)
				{
					list.Add(new RedirectResponse
					{
						RequestUri = new Uri(parent.Url),
						StatusCode = (HttpStatusCode)parentResponse.Status,
						TimeToHeaders = TimeSpan.FromMilliseconds(parent.Timing.ResponseStart >= 0 ?
							parent.Timing.ResponseStart : responseStart)
					});
					parent = parent.RedirectedFrom;
				}
				else
				{
					break;
				}
			}

			list.Reverse();
			return list;
		}

		private static async Task<HttpResponseMessage> ConvertIResponseToHttpResponse(IResponse playwrightResponse)
		{
			var httpResponse = new HttpResponseMessage((HttpStatusCode)playwrightResponse.Status)
			{
				Content = new System.Net.Http.ByteArrayContent(await playwrightResponse.BodyAsync()),
				RequestMessage = new HttpRequestMessage(new HttpMethod(playwrightResponse.Request.Method), playwrightResponse.Request.Url)
			};

			foreach (var header in playwrightResponse.Headers)
			{
				httpResponse.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
				httpResponse.Headers.TryAddWithoutValidation(header.Key, header.Value);
			}

			return httpResponse;
		}

		public string Name => Downloaders.Playwright;
	}
}
