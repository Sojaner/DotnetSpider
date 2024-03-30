using System.Threading.Tasks;
using LucasSpider.Http;

namespace LucasSpider.Downloader
{
	public interface IDownloader
	{
		Task<Response> DownloadAsync(Request request);

		string Name { get; }
	}
}
