using Microsoft.Extensions.Configuration;

namespace LucasSpider.Mongo
{
	public class MongoOptions
	{
		private readonly IConfiguration _configuration;

		public MongoOptions(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string ConnectionString => _configuration["Mongo:ConnectionString"];
	}
}
