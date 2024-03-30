using LucasSpider.DataFlow;
using LucasSpider.DataFlow.Storage;
using LucasSpider.Mongo;
using LucasSpider.MySql;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace LucasSpider.Tests
{
	public class StorageTests
	{
		[Fact]
		public void CreateDefaultMySqlStorage()
		{
			var configurationBuilder = new ConfigurationBuilder();
			configurationBuilder.AddJsonFile("appsettings.json");
			var configuration = configurationBuilder.Build();
			var storage =
				StorageUtilities.CreateStorage("LucasSpider.MySql.MySqlEntityStorage, LucasSpider.MySql",
					configuration) as MySqlEntityStorage;
			Assert.NotNull(storage);
			Assert.Equal(StorageMode.InsertIgnoreDuplicate, storage.Mode);
			Assert.Equal("Database='mysql2';Data Source=localhost;password=1qazZAQ!;User ID=root;Port=3307;",
				storage.ConnectionString);
			Assert.False(storage.IgnoreCase);
			Assert.Equal(1000, storage.RetryTimes);
			Assert.True(storage.UseTransaction);
		}

		[Fact]
		public void CreateDefaultSqlServerStorage()
		{
			var configurationBuilder = new ConfigurationBuilder();
			configurationBuilder.AddJsonFile("appsettings.json");
			var configuration = configurationBuilder.Build();
			var storage =
				StorageUtilities.CreateStorage("LucasSpider.DataFlow.SqlServerEntityStorage, LucasSpider",
					configuration) as SqlServerEntityStorage;
			Assert.NotNull(storage);
			Assert.Equal(StorageMode.InsertIgnoreDuplicate, storage.Mode);
			Assert.Equal("Database='mysql3';Data Source=localhost;password=1qazZAQ!;User ID=root;Port=3308;",
				storage.ConnectionString);
			Assert.False(storage.IgnoreCase);
			Assert.Equal(1000, storage.RetryTimes);
			Assert.True(storage.UseTransaction);
		}

		[Fact]
		public void CreateDefaultMongoStorage()
		{
			var configurationBuilder = new ConfigurationBuilder();
			configurationBuilder.AddJsonFile("appsettings.json");
			var configuration = configurationBuilder.Build();
			var storage =
				StorageUtilities.CreateStorage("LucasSpider.Mongo.MongoEntityStorage, LucasSpider.Mongo",
					configuration) as MongoEntityStorage;
			Assert.NotNull(storage);

			Assert.Equal("mongodb://user1:password1@localhost/test",
				storage.ConnectionString);
		}
	}
}
