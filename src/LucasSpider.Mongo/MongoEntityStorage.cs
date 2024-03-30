using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LucasSpider.DataFlow;
using LucasSpider.DataFlow.Storage;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

[assembly: InternalsVisibleTo("LucasSpider.Tests")]

namespace LucasSpider.Mongo
{
	/// <summary>
	/// MongoDB saves parsing (entity) results TODO: Should we consider the storage mode: insert, new insert, old update, update ETC
	/// </summary>
	public class MongoEntityStorage : EntityStorageBase
	{
		private readonly IMongoClient _client;

		private readonly ConcurrentDictionary<Type, TableMetadata> _tableMetadataDict =
			new();

		private readonly ConcurrentDictionary<string, IMongoDatabase> _cache =
			new();

		public static IDataFlow CreateFromOptions(IConfiguration configuration)
		{
			var options = new MongoOptions(configuration);
			return new MongoEntityStorage(options.ConnectionString);
		}

		public string ConnectionString { get; }

		/// <summary>
		/// Construction method
		/// </summary>
		/// <param name="connectionString">Connection string</param>
		public MongoEntityStorage(string connectionString)
		{
			_client = new MongoClient(connectionString);
			ConnectionString = connectionString;
		}

		internal MongoEntityStorage(IMongoClient mongoClient)
		{
			_client = mongoClient;
		}

		public override Task InitializeAsync()
		{
			return Task.CompletedTask;
		}

		protected override async Task HandleAsync(DataFlowContext context,
			IDictionary<Type, ICollection<dynamic>> entities)
		{
			foreach (var kv in entities)
			{
				var list = (IList)kv.Value;
				var tableMetadata = _tableMetadataDict.GetOrAdd(kv.Key,
					_ => ((IEntity)list[0]).GetTableMetadata());

				if (string.IsNullOrWhiteSpace(tableMetadata.Schema.Database))
				{
					throw new ArgumentException("Database of schema should not be empty or null");
				}

				if (!_cache.ContainsKey(tableMetadata.Schema.Database))
				{
					_cache.TryAdd(tableMetadata.Schema.Database, _client.GetDatabase(tableMetadata.Schema.Database));
				}

				var db = _cache[tableMetadata.Schema.Database];
				var collection = db.GetCollection<BsonDocument>(tableMetadata.Schema.Table);

				BsonSerializer
					.RegisterSerializer(new ObjectSerializer(type =>
						ObjectSerializer.DefaultAllowedTypes(type) ||
						list.Cast<object>().Any(o => o.GetType().FullName == type.FullName)));

				var bsonDocs = new List<BsonDocument>();
				foreach (var data in list)
				{
					bsonDocs.Add(data.ToBsonDocument());
				}

				await collection.InsertManyAsync(bsonDocs);
			}
		}

		public override string ToString()
		{
			return $"{ConnectionString}";
		}
	}
}
