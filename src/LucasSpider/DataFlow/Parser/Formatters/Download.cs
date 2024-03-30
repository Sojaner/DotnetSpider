using System;
using System.IO;
using System.Net;

namespace LucasSpider.DataFlow.Parser.Formatters
{
	/// <summary>
	/// Download content
	/// </summary>
	public class Download : Formatter
	{
		private readonly WebClient _client = new();
		/// <summary>
		/// Perform download operation
		/// </summary>
		/// <param name="value">Download link</param>
		/// <returns>File name after download is completed</returns>
		protected override string Handle(string value)
		{
			var filePath = value;
			var name = Path.GetFileName(filePath);
			var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", name);
			_client.DownloadFile(file, filePath);
			return file;
		}

		/// <summary>
		/// Verify whether the parameters are set correctly
		/// </summary>
		protected override void CheckArguments()
		{
		}
	}
}
