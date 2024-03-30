using System;
using LucasSpider.DataFlow.Storage;
using Microsoft.Extensions.Configuration;

namespace LucasSpider.PostgreSql
{
	public class PostgreOptions
	{
		private readonly IConfiguration _configuration;

		public PostgreOptions(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public StorageMode Mode => string.IsNullOrWhiteSpace(_configuration["Postgre:Mode"])
			? StorageMode.Insert
			: (StorageMode)Enum.Parse(typeof(StorageMode), _configuration["Postgre:Mode"]);

		/// <summary>
		/// Database connection string
		/// </summary>
		public string ConnectionString => _configuration["Postgre:ConnectionString"];

		/// <summary>
		/// Number of database operation retries
		/// </summary>
		public int RetryTimes => string.IsNullOrWhiteSpace(_configuration["Postgre:RetryTimes"])
			? 600
			: int.Parse(_configuration["Postgre:RetryTimes"]);

		/// <summary>
		/// Whether to use transaction operations. 
		/// </summary>
		public bool UseTransaction => !string.IsNullOrWhiteSpace(_configuration["Postgre:UseTransaction"]) &&
		                              bool.Parse(_configuration["Postgre:UseTransaction"]);

		/// <summary>
		/// Database ignores case
		/// </summary>
		public bool IgnoreCase => !string.IsNullOrWhiteSpace(_configuration["Postgre:IgnoreCase"]) &&
		                          bool.Parse(_configuration["Postgre:IgnoreCase"]);
	}
}
