using System;
using LucasSpider.Extensions;
using LucasSpider.Http;

namespace LucasSpider.Infrastructure
{
	/// <summary>
	///Request hash compiler
	/// </summary>
	public class RequestHasher : IRequestHasher
	{
		private readonly IHashAlgorithmService _hashAlgorithmService;

		public RequestHasher(IHashAlgorithmService hashAlgorithmService)
		{
			_hashAlgorithmService = hashAlgorithmService;
		}

		public void ComputeHash(Request request)
		{
			var bytes = new
			{
				request.Owner,
				request.RequestUri.AbsoluteUri,
				request.Method,
				request.RequestedTimes,
				request.Content
			}.Serialize();
			request.Hash = Convert.ToBase64String(_hashAlgorithmService.ComputeHash(bytes)).TrimEnd('=');
			// return request.Hash;
		}
	}
}
