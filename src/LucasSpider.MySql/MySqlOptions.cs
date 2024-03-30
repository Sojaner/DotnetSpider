using System;
using LucasSpider.DataFlow.Storage;
using Microsoft.Extensions.Configuration;

namespace LucasSpider.MySql
{
	public class MySqlOptions
	{
		private readonly IConfiguration _configuration;

		public MySqlOptions(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public StorageMode Mode => string.IsNullOrWhiteSpace(_configuration["MySql:Mode"])
			? StorageMode.Insert
			: (StorageMode)Enum.Parse(typeof(StorageMode), _configuration["MySql:Mode"]);

		/// <summary>
		/// Database connection string
		/// </summary>
		public string ConnectionString => _configuration["MySql:ConnectionString"];

		/// <summary>
		/// Number of database operation retries
		/// </summary>
		public int RetryTimes => string.IsNullOrWhiteSpace(_configuration["MySql:RetryTimes"])
			? 600
			: int.Parse(_configuration["MySql:RetryTimes"]);

		/// <summary>
		/// Whether to use transaction operations. 
		/// </summary>
		public bool UseTransaction => !string.IsNullOrWhiteSpace(_configuration["MySql:UseTransaction"]) &&
		                              bool.Parse(_configuration["MySql:UseTransaction"]);

		/// <summary>
		/// Database ignores case
		/// </summary>
		public bool IgnoreCase => !string.IsNullOrWhiteSpace(_configuration["MySql:IgnoreCase"]) &&
		                          bool.Parse(_configuration["MySql:IgnoreCase"]);
	}
}
