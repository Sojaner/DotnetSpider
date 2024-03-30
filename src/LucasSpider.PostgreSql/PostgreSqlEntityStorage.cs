using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LucasSpider.DataFlow;
using LucasSpider.DataFlow.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace LucasSpider.PostgreSql
{
	/// <summary>
	/// PostgreSql saves parsing (entity) results
	/// </summary>
	public class PostgreSqlEntityStorage : RelationalDatabaseEntityStorageBase
	{
		/// <summary>
		/// Return to memory according to configuration ¬
		/// </summary>
		/// <param name="configuration">Deployment</param>
		/// <returns></returns>
		public static IDataFlow CreateFromOptions(IConfiguration configuration)
		{
			var options = new PostgreOptions(configuration);
			return new PostgreSqlEntityStorage(options.Mode, options.ConnectionString)
			{
				UseTransaction = options.UseTransaction,
				IgnoreCase = options.IgnoreCase,
				RetryTimes = options.RetryTimes
			};
		}

		public override Task InitializeAsync()
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Create database and tables
		/// </summary>
		/// <param name="conn">Database connection</param>
		/// <param name="sqlStatements">SQL statements</param>
		protected override void EnsureDatabaseAndTableCreated(IDbConnection conn,
			SqlStatements sqlStatements)
		{
			if (!string.IsNullOrWhiteSpace(sqlStatements.CreateDatabaseSql))
			{
				try
				{
					conn.Execute(sqlStatements.CreateDatabaseSql);
				}
				catch (Exception e)
				{
					if (e.Message != $"42P04: database {sqlStatements.DatabaseSql} already exists")
					{
						throw;
					}
				}
			}

			conn.Execute(sqlStatements.CreateTableSql);
		}

		protected virtual string GenerateCreateTableSql(TableMetadata tableMetadata)
		{
			var isAutoIncrementPrimary = tableMetadata.IsAutoIncrementPrimary;

			var tableSql = GenerateTableSql(tableMetadata);

			var builder = new StringBuilder($"CREATE TABLE IF NOT EXISTS {tableSql} (");

			foreach (var column in tableMetadata.Columns)
			{
				var isPrimary = tableMetadata.IsPrimary(column.Key);

				if (isPrimary)
				{
					var primarySql = $"CONSTRAINT {GetTableName(tableMetadata).ToUpper()}_PK PRIMARY KEY, ";
					builder.Append(isAutoIncrementPrimary
						? $"{GetNameSql(column.Value.Name)} SERIAL {primarySql}"
						: $"{GenerateColumnSql(column.Value, true)} {(tableMetadata.Primary.Count > 1 ? "" : primarySql)}");
				}
				else
				{
					builder.Append($"{GenerateColumnSql(column.Value, false)}, ");
				}
			}

			builder.Remove(builder.Length - 2, 2);

			if (tableMetadata.Primary != null && tableMetadata.Primary.Count > 1)
			{
				builder.Append(
					$", CONSTRAINT {GetTableName(tableMetadata).ToUpper()}_PK PRIMARY KEY ({string.Join(", ", tableMetadata.Primary.Select(c => $"{Escape}{GetNameSql(c)}{Escape}"))})");
			}

			if (tableMetadata.Indexes.Count > 0)
			{
				foreach (var index in tableMetadata.Indexes.Where(x => x.IsUnique))
				{
					var name = index.Name;
					var columnNames = string.Join(", ", index.Columns.Select(c => $"{Escape}{GetNameSql(c)}{Escape}"));
					builder.Append(
						$", CONSTRAINT {Escape}{name}{Escape} UNIQUE ({columnNames})");
				}
			}

			builder.Append(");");
			if (tableMetadata.Indexes.Count > 0)
			{
				foreach (var index in tableMetadata.Indexes.Where(x => x.IsUnique))
				{
					var name = index.Name;
					var columnNames = string.Join(", ", index.Columns.Select(c => $"{Escape}{GetNameSql(c)}{Escape}"));
					builder.Append(
						$"CREATE INDEX {name} ON {tableSql} ({columnNames});");
				}
			}

			var sql = builder.ToString();
			return sql;
		}

		protected override IDbConnection CreateDbConnection(string connectString)
		{
			return new NpgsqlConnection(connectString);
		}

		protected virtual string Escape => "\"";

		/// <summary>
		/// Construction method
		/// </summary>
		/// <param name="mode">Memory type</param>
		/// <param name="connectionString">Connection string</param>
		public PostgreSqlEntityStorage(StorageMode mode,
			string connectionString) : base(mode,
			connectionString)
		{
		}

		protected virtual string GenerateCreateDatabaseSql(TableMetadata tableMetadata)
		{
			return string.IsNullOrWhiteSpace(tableMetadata.Schema.Database)
				? ""
				: $"CREATE DATABASE {Escape}{GetNameSql(tableMetadata.Schema.Database)}{Escape} with encoding 'UTF-8';";
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
					dataType = "BOOL";
					break;
				}
				case DateTimeType:
				{
					dataType = "TIMESTAMP DEFAULT CURRENT_TIMESTAMP";
					break;
				}
				case DateTimeOffsetType:
				{
					dataType = "TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP";
					break;
				}
				case DecimalType:
				{
					dataType = "NUMERIC";
					break;
				}
				case DoubleType:
				{
					dataType = "FLOAT8";
					break;
				}
				case FloatType:
				{
					dataType = "FLOAT4";
					break;
				}
				case IntType:
				{
					dataType = "INT4";
					break;
				}
				case LongType:
				{
					dataType = "INT8";
					break;
				}
				case ByteType:
				{
					dataType = "INT2";
					break;
				}
				case ShortType:
				{
					dataType = "INT2";
					break;
				}
				default:
				{
					dataType = length <= 0 || length > 8000 ? "TEXT" : $"VARCHAR({length})";
					break;
				}
			}

			return dataType;
		}

		protected override SqlStatements GenerateSqlStatements(TableMetadata tableMetadata)
		{
			var sqlStatements = new SqlStatements
			{
				InsertSql = GenerateInsertSql(tableMetadata, false),
				InsertIgnoreDuplicateSql = GenerateInsertSql(tableMetadata, true),
				InsertAndUpdateSql = GenerateInsertAndUpdateSql(tableMetadata),
				UpdateSql = GenerateUpdateSql(tableMetadata),
				CreateTableSql = GenerateCreateTableSql(tableMetadata),
				CreateDatabaseSql = GenerateCreateDatabaseSql(tableMetadata),
				DatabaseSql = string.IsNullOrWhiteSpace(tableMetadata.Schema.Database)
					? ""
					: $"{Escape}{GetNameSql(tableMetadata.Schema.Database)}{Escape}"
			};
			return sqlStatements;
		}

		/// <summary>
		/// SQL to generate database name
		/// </summary>
		/// <param name="tableMetadata">Table metadata</param>
		/// <returns>SQL statement</returns>
		protected virtual string GenerateTableSql(TableMetadata tableMetadata)
		{
			var tableName = GetNameSql(GetTableName(tableMetadata));
			var database = GetNameSql(tableMetadata.Schema.Database);
			return string.IsNullOrWhiteSpace(database)
				? $"{Escape}{tableName}{Escape}"
				: $"{Escape}{database}{Escape}.{Escape}{tableName}{Escape}";
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

			return $"{Escape}{columnName}{Escape} {dataType}";
		}

		/// <summary>
		/// Generate SQL statements to insert data
		/// </summary>
		/// <param name="tableMetadata">Table metadata</param>
		/// <param name="ignoreDuplicate">Whether to ignore data with duplicate keys</param>
		/// <returns>SQL statement</returns>
		protected virtual string GenerateInsertSql(TableMetadata tableMetadata, bool ignoreDuplicate)
		{
			var columns = tableMetadata.Columns;
			var isAutoIncrementPrimary = tableMetadata.IsAutoIncrementPrimary;
			// Remove the auto-incrementing primary key
			var insertColumns =
				(isAutoIncrementPrimary ? columns.Where(c1 => c1.Key != tableMetadata.Primary.First()) : columns)
				.ToArray();

			var columnsSql = string.Join(", ", insertColumns.Select(c => $"{Escape}{GetNameSql(c.Key)}{Escape}"));

			var columnsParamsSql = string.Join(", ", insertColumns.Select(p => $"@{p.Key}"));

			var tableSql = GenerateTableSql(tableMetadata);

			var sql =
				$"INSERT {(ignoreDuplicate ? "IGNORE" : "")} INTO {tableSql} ({columnsSql}) VALUES ({columnsParamsSql});";
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
				Logger?.LogWarning("The entity does not have a primary key set and cannot generate an Update statement.");
				return null;
			}

			var where = "";
			foreach (var column in tableMetadata.Primary)
			{
				where += $" {Escape}{GetNameSql(column)}{Escape} = @{column} AND";
			}

			where = where.Substring(0, where.Length - 3);

			var setCols = string.Join(", ",
				tableMetadata.Updates.Select(c => $"{Escape}{GetNameSql(c)}{Escape}= @{c}"));
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

			var columns = tableMetadata.Columns;
			var isAutoIncrementPrimary = tableMetadata.IsAutoIncrementPrimary;
			// Remove the auto-incrementing primary key
			var insertColumns =
				(isAutoIncrementPrimary ? columns.Where(c1 => c1.Key != tableMetadata.Primary.First()) : columns)
				.ToArray();

			var columnsSql = string.Join(", ", insertColumns.Select(c => $"{Escape}{GetNameSql(c.Key)}{Escape}"));

			var columnsParamsSql = string.Join(", ", insertColumns.Select(p => $"@{p.Key}"));

			var tableSql = GenerateTableSql(tableMetadata);
			var setCols = string.Join(", ",
				insertColumns.Select(c => $"{Escape}{GetNameSql(c.Key)}{Escape}= @{c.Key}"));
			var sql =
				$"INSERT INTO {tableSql} ({columnsSql}) VALUES ({columnsParamsSql}) ON DUPLICATE key UPDATE {setCols};";
			return sql;
		}
	}
}
