using System;

namespace LucasSpider.Http
{
	public interface IHttpContent : IDisposable, ICloneable
	{
		ContentHeaders Headers { get; }
	}
}
