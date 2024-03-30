using System;

namespace LucasSpider.Infrastructure
{
	public readonly struct SpiderId
	{
		public readonly string Id;
		public readonly string Name;

		public SpiderId(string id, string name)
		{
			id.NotNullOrWhiteSpace("Id");
			if (id.Length > 36)
			{
				throw new ArgumentException("Id length cannot exceed 36 characters");
			}

			Id = id;
			Name = name;
		}

		public override string ToString()
		{
			return Id;
		}
	}
}
