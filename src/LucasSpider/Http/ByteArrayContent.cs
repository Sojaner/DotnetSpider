using System;
using System.Text.Json.Serialization;

namespace LucasSpider.Http
{
	public class ByteArrayContent : IHttpContent
	{
		[JsonInclude]
		private ContentHeaders _headers;
		private bool _disposed;

		public ContentHeaders Headers => _headers ??= new ContentHeaders();

		/// <summary>
		/// Content
		/// </summary>
		public byte[] Bytes { get; private set; }

		public ByteArrayContent(byte[] bytes)
		{
			Bytes = bytes;
		}

		public object Clone()
		{
			var bytes = new byte[Bytes.Length];
			Bytes.CopyTo(bytes, 0);

			var content = new ByteArrayContent(bytes);

			if (_headers != null)
			{
				foreach (var header in _headers)
				{
					content.Headers.Add(header.Key, header.Value);
				}
			}

			return content;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing || _disposed)
			{
				return;
			}

			_disposed = true;
			if (_headers != null)
			{
				_headers.Clear();
				_headers = null;
			}


			Bytes = null;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
