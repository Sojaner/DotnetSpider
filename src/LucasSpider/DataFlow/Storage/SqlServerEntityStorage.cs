using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LucasSpider.DataFlow.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace LucasSpider.DataFlow
{
	/// <summary>
	/// Database version
	/// </summary>
	public enum SqlServerVersion
	{
		V2000,
		V2005,
		V2008,
		V2012,
		V2017
	}

	public class SqlServerOptions
	{
		private readonly IConfiguration _configuration;

		public SqlServerOptions(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public StorageMode Mode => string.IsNullOrWhiteSpace(_configuration["SqlServer:Mode"])
			? StorageMode.Insert
			: (StorageMode)Enum.Parse(typeof(StorageMode), _configuration["SqlServer:Mode"]);

		public SqlServerVersion ServerVersion => string.IsNullOrWhiteSpace(_configuration["SqlServer:Version"])
			? SqlServerVersion.V2008
			: (SqlServerVersion)Enum.Parse(typeof(SqlServerVersion), _configuration["SqlServer:Version"]);

		/// <summary>
		/// Connection string
		/// </summary>
		public string ConnectionString => _configuration["SqlServer:ConnectionString"];

		/// <summary>
		/// Number of database operation retries
		/// </summary>
		public int RetryTimes => string.IsNullOrWhiteSpace(_configuration["SqlServer:RetryTimes"])
			? 600
			: int.Parse(_configuration["SqlServer:RetryTimes"]);

		/// <summary>
		/// Whether to use transaction operations.
		/// </summary>
		public bool UseTransaction => !string.IsNullOrWhiteSpace(_configuration["SqlServer:UseTransaction"]) &&
		                              bool.Parse(_configuration["SqlServer:UseTransaction"]);

		/// <summary>
		/// Database ignores case
		/// </summary>
		public bool IgnoreCase => !string.IsNullOrWhiteSpace(_configuration["SqlServer:IgnoreCase"]) &&
		                          bool.Parse(_configuration["SqlServer:IgnoreCase"]);
	}

	/// <summary>
	/// SqlServer saves parsing (entity) results
	/// </summary>
	public class SqlServerEntityStorage : RelationalDatabaseEntityStorageBase
	{
		private readonly SqlServerVersion _serverVersion;

		public static IDataFlow CreateFromOptions(IConfiguration configuration)
		{
			var options = new SqlServerOptions(configuration);
			return new SqlServerEntityStorage(options.Mode, options.ConnectionString, options.ServerVersion)
			{
				RetryTimes = options.RetryTimes,
				UseTransaction = options.UseTransaction,
				IgnoreCase = options.IgnoreCase
			};
		}

		public override Task InitializeAsync()
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Construction method
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="connectionString">Connection string</param>
		/// <param name="version">Database version</param>
		public SqlServerEntityStorage(StorageMode mode,
			string connectionString, SqlServerVersion version = SqlServerVersion.V2017) : base(mode,
			connectionString)
		{
			_serverVersion = version;
		}

		/// <summary>
		/// Create database connection interface
		/// </summary>
		/// <param name="connectString">Connection string</param>
		/// <returns></returns>
		protected override IDbConnection CreateDbConnection(string connectString)
		{
			return new SqlConnection(connectString);
		}

		/// <summary>
		/// Generate SQL statements
		/// </summary>
		/// <param name="tableMetadata">Table metadata</param>
		/// <returns></returns>
		protected override SqlStatements GenerateSqlStatements(TableMetadata tableMetadata)
		{
			var sqlStatements = new SqlStatements
			{
				InsertSql = GenerateInsertSql(tableMetadata),
				InsertIgnoreDuplicateSql = GenerateInsertSql(tableMetadata),
				InsertAndUpdateSql = GenerateInsertAndUpdateSql(tableMetadata),
				UpdateSql = GenerateUpdateSql(tableMetadata),
				CreateTableSql = GenerateCreateTableSql(tableMetadata),
				CreateDatabaseSql = GenerateCreateDatabaseSql(tableMetadata),
				DatabaseSql = string.IsNullOrWhiteSpace(tableMetadata.Schema.Database)
					? ""
					: $"[{GetNameSql(tableMetadata.Schema.Database)}]"
			};
			return sqlStatements;
		}

		/// <summary>
		/// Generate SQL statements to create the database
		/// </summary>
		/// <param name="tableMetadata">Table metadata</param>
		/// <returns>SQL statement</returns>
		protected virtual string GenerateCreateDatabaseSql(TableMetadata tableMetadata)
		{
			if (string.IsNullOrWhiteSpace(tableMetadata.Schema.Database))
			{
				return null;
			}

			var database = GetNameSql(tableMetadata.Schema.Database);
			switch (_serverVersion)
			{
				case SqlServerVersion.V2000:
				case SqlServerVersion.V2005:
				case SqlServerVersion.V2008:
				{
					return
						$"USE master; IF NOT EXISTS(SELECT * FROM sys.databases WHERE name='{database}') CREATE DATABASE {database};";
				}
				case SqlServerVersion.V2012:
				case SqlServerVersion.V2017:
				{
					return
						$"USE master; IF NOT EXISTS(SELECT * FROM sysdatabases WHERE name='{database}') CREATE DATABASE {database};";
				}
				default:
				{
					return
						$"USE master; IF NOT EXISTS(SELECT * FROM sysdatabases WHERE name='{database}') CREATE DATABASE {database};";
				}
			}
		}

		/// <summary>
		/// Generate SQL statements to create tables
		/// </summary>
		/// <param name="tableMetadata">Table metadata</param>
		/// <returns>SQL statement</returns>
		protected virtual string GenerateCreateTableSql(TableMetadata tableMetadata)
		{
			var isAutoIncrementPrimary = tableMetadata.IsAutoIncrementPrimary;

			var tableName = GetNameSql(tableMetadata.Schema.Table);
			var database = GetNameSql(tableMetadata.Schema.Database);

			var builder = string.IsNullOrWhiteSpace(database)
				? new StringBuilder($"IF OBJECT_ID('{tableName}', 'U') IS NULL BEGIN CREATE table {tableName} (")
				: new StringBuilder(
					$"USE {database}; IF OBJECT_ID('{tableName}', 'U') IS NULL BEGIN CREATE table {tableName} (");

			foreach (var column in tableMetadata.Columns)
			{
				var isPrimary = tableMetadata.IsPrimary(column.Key);

				var columnSql = GenerateColumnSql(column.Value, isPrimary);

				if (isAutoIncrementPrimary && isPrimary)
				{
					builder.Append($"{columnSql} IDENTITY(1,1), ");
				}
				else
				{
					builder.Append($"{columnSql}, ");
				}
			}

			builder.Remove(builder.Length - 2, 2);

			if (tableMetadata.HasPrimary)
			{
				var primaryKeys = string.Join(", ", tableMetadata.Primary.Select(c => $"[{GetNameSql(c)}]"));
				builder.Append(
					$", CONSTRAINT [PK_{tableName}] PRIMARY KEY CLUSTERED ({primaryKeys}) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = ON , ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY];");
			}
			else
			{
				builder.Append(") ON [PRIMARY];");
			}

			if (tableMetadata.Indexes.Count > 0)
			{
				foreach (var index in tableMetadata.Indexes)
				{
					var name = index.Name;
					var columnNames = string.Join(", ", index.Columns.Select(c => $"[{GetNameSql(c)}]"));
					if (index.IsUnique)
					{
						builder.Append(
							$"CREATE UNIQUE NONCLUSTERED INDEX [INDEX_{name}] ON {tableName} ({columnNames}) {(Mode == StorageMode.InsertIgnoreDuplicate ? "WITH (IGNORE_DUP_KEY = ON)" : "")};");
					}
					else
					{
						builder.Append(
							$"CREATE NONCLUSTERED INDEX [INDEX_{name}] ON {tableName} ({columnNames});");
					}
				}
			}

			builder.AppendLine(" END");
			var sql = builder.ToString();
			return sql;
		}

		/// <summary>
		/// Generate SQL statements to insert data
		/// </summary>
		/// <param name="tableMetadata">Table metadata</param>
		/// <returns>SQL statement</returns>
		protected virtual string GenerateInsertSql(TableMetadata tableMetadata)
		{
			var columns = tableMetadata.Columns;
			var isAutoIncrementPrimary = tableMetadata.IsAutoIncrementPrimary;
			// Remove the auto-incrementing primary key
			var insertColumns =
				(isAutoIncrementPrimary ? columns.Where(c1 => c1.Key != tableMetadata.Primary.First()) : columns)
				.ToArray();

			var columnsSql = string.Join(", ", insertColumns.Select(c => $"[{GetNameSql(c.Key)}]"));

			var columnsParamsSql = string.Join(", ", insertColumns.Select(p => $"@{p.Key}"));

			var tableSql = GenerateTableSql(tableMetadata);

			var sql = $"INSERT INTO {tableSql} ({columnsSql}) VALUES ({columnsParamsSql});";
			return sql;
		}

		/// <summary>
		/// Generate SQL statements to update data
		/// </summary>
		/// <param name="tableMetadata">Table metadata</param>
		/// <returns>SQL statement</returns>
		protected virtual string GenerateUpdateSql(TableMetadata tableMetadata)
		{
			if (tableMetadata.Updates == null || tableMetadata.Updates.Count == 0)
			{
				return null;
			}

			var where = "";
			foreach (var column in tableMetadata.Primary)
			{
				where += $" [{GetNameSql(column)}] = @{column} AND";
			}

			where = where.Substring(0, where.Length - 3);

			var setCols = string.Join(", ", tableMetadata.Updates.Select(c => $"[{GetNameSql(c)}] = @{c}"));

			var tableSql = GenerateTableSql(tableMetadata);
			var sql = $"UPDATE {tableSql} SET {setCols} WHERE {where};";
			return sql;
		}

		/// <summary>
		/// Generate SQL statements to insert new data or update old data
		/// </summary>
		/// <param name="tableMetadata">Table metadata</param>
		/// <returns>SQL statement</returns>
		protected virtual string GenerateInsertAndUpdateSql(TableMetadata tableMetadata)
		{
			if (!tableMetadata.HasPrimary)
			{
				Logger?.LogWarning("The entity does not have a primary key set, and the InsertAndUpdate statement cannot be generated.");
				return null;
			}

			// UPDATE MyTable SET FieldA=@FieldA WHERE Key=@Key IF @@ROWCOUNT = 0 INSERT INTO MyTable (FieldA) VALUES (@FieldA)
			var columns = tableMetadata.Columns;
			var isAutoIncrementPrimary = tableMetadata.IsAutoIncrementPrimary;
			// Remove the auto-incrementing primary key
			var insertColumns =
				(isAutoIncrementPrimary ? columns.Where(c1 => c1.Key != tableMetadata.Primary.First()) : columns)
				.ToArray();

			var columnsSql = string.Join(", ", insertColumns.Select(c => $"[{GetNameSql(c.Key)}]"));

			var columnsParamsSql = string.Join(", ", insertColumns.Select(p => $"@{p.Key}"));

			var where = "";
			foreach (var column in tableMetadata.Primary)
			{
				where += $" [{GetNameSql(column)}] = @{column} AND";
			}

			where = where.Substring(0, where.Length - 3);

			var tableSql = GenerateTableSql(tableMetadata);

			var setCols = string.Join(", ", insertColumns.Select(c => $"[{GetNameSql(c.Key)}] = @{c.Key}"));
			var sql =
				$"UPDATE {tableSql} SET {setCols} WHERE {where} IF @@ROWCOUNT = 0 INSERT INTO {tableSql} ({columnsSql}) VALUES ({columnsParamsSql});";
			return sql;
		}

		/// <summary>
		/// SQL to generate database name
		/// </summary>
		/// <param name="tableMetadata">Table metadata</param>
		/// <returns>SQL statement</returns>
		protected virtual string GenerateTableSql(TableMetadata tableMetadata)
		{
			var tableName = GetNameSql(tableMetadata.Schema.Table);
			var database = GetNameSql(tableMetadata.Schema.Database);
			return string.IsNullOrWhiteSpace(database) ? $"[{tableName}]" : $"[{database}].[dbo].[{tableName}]";
		}

		/// <summary>
		/// SQL to generate columns
		/// </summary>
		/// <returns>SQL statement</returns>
		protected virtual string GenerateColumnSql(Column column, bool isPrimary)
		{
			var columnName = GetNameSql(column.Name);
			var dataType = GenerateDataTypeSql(column.Type, column.Length);
			if (isPrimary || column.Required)
			{
				dataType = $"{dataType} NOT NULL";
			}

			return $"[{columnName}] {dataType}";
		}

		/// <summary>
		/// Generate SQL for data types
		/// </summary>
		/// <param name="type">Data type</param>
		/// <param name="length">Data length</param>
		/// <returns>SQL statement</returns>
		protected virtual string GenerateDataTypeSql(string type, int length)
		{
			string dataType;

			switch (type)
			{
				case BoolType:
				{
					dataType = "BIT";
					break;
				}
				case DateTimeType:
				case DateTimeOffsetType:
				{
					dataType = "DATETIME DEFAULT(GETDATE())";
					break;
				}

				case DecimalType:
				{
					dataType = "DECIMAL(18,2)";
					break;
				}
				case DoubleType:
				{
					dataType = "FLOAT";
					break;
				}
				case FloatType:
				{
					dataType = "FLOAT";
					break;
				}
				case IntType:
				{
					dataType = "INT";
					break;
				}
				case LongType:
				{
					dataType = "BIGINT";
					break;
				}
				case ByteType:
				{
					dataType = "INT";
					break;
				}
				default:
				{
					dataType = length <= 0 || length > 8000 ? "NVARCHAR(MAX)" : $"VARCHAR({length})";
					break;
				}
			}

			return dataType;
		}
	}
}
