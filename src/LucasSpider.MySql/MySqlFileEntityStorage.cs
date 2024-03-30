using System;
using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LucasSpider.DataFlow;
using LucasSpider.DataFlow.Storage;
using LucasSpider.Infrastructure;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace LucasSpider.MySql
{
	public class MySqlFileOptions
	{
		private readonly IConfiguration _configuration;

		public MySqlFileOptions(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public MySqlFileType Type => string.IsNullOrWhiteSpace(_configuration["MySqlFile:Type"])
			? MySqlFileType.LoadFile
			: (MySqlFileType)Enum.Parse(typeof(MySqlFileType), _configuration["MySqlFile:Type"]);

		public bool IgnoreCase => string.IsNullOrWhiteSpace(_configuration["MySqlFile:IgnoreCase"]) ||
		                          bool.Parse(_configuration["MySqlFile:IgnoreCase"]);
	}

	/// <summary>
	/// Save the parsed crawler entity data into a SQL file, supporting two modes
	/// LoadFile is the batch import mode through the command LOAD DATA LOCAL INFILE '{filePath}' INTO TABLE `{schema}`.`{dababase}` FIELDS TERMINATED BY '$' ENCLOSED BY '#' LINES TERMINATED BY '@END@' IGNORE 1 
	/// InsertSql is a complete Insert SQL statement, which needs to be executed one by one to import data.
	/// </summary>
	public class MySqlFileEntityStorage : EntityFileStorageBase
	{
		private readonly ConcurrentDictionary<string, StreamWriter> _streamWriters =
			new();

		/// <summary>
		/// Database ignores case
		/// </summary>
		public bool IgnoreCase { get; set; }

		public MySqlFileType MySqlFileType { get; set; }

		public override Task InitializeAsync()
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Returns memory based on configuration
		/// </summary>
		/// <param name="configuration">Deployment</param>
		/// <returns></returns>
		public static MySqlFileEntityStorage CreateFromOptions(IConfiguration configuration)
		{
			var options = new MySqlFileOptions(configuration);
			return new MySqlFileEntityStorage(options.Type, options.IgnoreCase);
		}

		/// <summary>
		/// Construction method
		/// </summary>
		/// <param name="fileType">File type</param>
		/// <param name="ignoreCase"></param>
		public MySqlFileEntityStorage(MySqlFileType fileType = MySqlFileType.LoadFile, bool ignoreCase = false)
		{
			MySqlFileType = fileType;
			IgnoreCase = ignoreCase;
		}

		protected override async Task HandleAsync(DataFlowContext context, TableMetadata tableMetadata, IEnumerable entities)
		{
			var writer = _streamWriters.GetOrAdd(tableMetadata.TypeName,
				_ => OpenWrite(context, tableMetadata, "sql"));

			switch (MySqlFileType)
			{
				case MySqlFileType.LoadFile:
				{
					await WriteLoadFileAsync(writer, tableMetadata, entities);
					break;
				}
				case MySqlFileType.InsertSql:
				{
					await WriteInsertFile(writer, tableMetadata, entities);
					break;
				}
			}
		}

		public override void Dispose()
		{
			foreach (var streamWriter in _streamWriters)
			{
				streamWriter.Value.Dispose();
			}

			base.Dispose();
		}

		private async Task WriteLoadFileAsync(StreamWriter writer, TableMetadata tableMetadata, IEnumerable items)
		{
			var builder = new StringBuilder();
			var columns = tableMetadata.Columns;
			var isAutoIncrementPrimary = tableMetadata.IsAutoIncrementPrimary;

			var insertColumns =
				(isAutoIncrementPrimary ? columns.Where(c1 => c1.Key != tableMetadata.Primary.First()) : columns)
				.ToArray();
			foreach (var item in items)
			{
				builder.Append("@END@");
				foreach (var column in insertColumns)
				{
					var value = column.Value.PropertyInfo.GetValue(item);
					value = value == null ? "" : MySqlHelper.EscapeString(value.ToString());
					builder.Append("#").Append(value).Append("#").Append("$");
				}
			}

			await writer.WriteLineAsync(builder.ToString());
		}

		private async Task WriteInsertFile(StreamWriter writer, TableMetadata tableMetadata, IEnumerable items)
		{
			var builder = new StringBuilder();
			var columns = tableMetadata.Columns;
			var isAutoIncrementPrimary = tableMetadata.IsAutoIncrementPrimary;
			var tableSql = GenerateTableSql(tableMetadata);

			var insertColumns =
				(isAutoIncrementPrimary ? columns.Where(c1 => c1.Key != tableMetadata.Primary.First()) : columns)
				.ToArray();
			foreach (var item in items)
			{
				builder.Append($"INSERT IGNORE INTO {tableSql} (");
				var lastColumn = insertColumns.Last();
				foreach (var column in insertColumns)
				{
					builder.Append(column.Equals(lastColumn) ? $"`{column.Key}`" : $"`{column.Key}`, ");
				}

				builder.Append(") VALUES (");

				foreach (var column in insertColumns)
				{
					var value = column.Value.PropertyInfo.GetValue(item);
					value = value == null ? "" : MySqlHelper.EscapeString(value.ToString());
					builder.Append(column.Equals(lastColumn) ? $"'{value}'" : $"'{value}', ");
				}

				builder.Append($");{Environment.NewLine}");
			}

			await writer.WriteLineAsync(builder.ToString());

			builder.Clear();
		}

		protected virtual string GenerateTableSql(TableMetadata tableMetadata)
		{
			var tableName = GetNameSql(tableMetadata.Schema.Table);
			var database = GetNameSql(tableMetadata.Schema.Database);
			return string.IsNullOrWhiteSpace(database) ? $"`{tableName}`" : $"`{database}`.`{tableName}`";
		}

		private string GetNameSql(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				return null;
			}

			return IgnoreCase ? name.ToLowerInvariant() : name;
		}
	}
}
