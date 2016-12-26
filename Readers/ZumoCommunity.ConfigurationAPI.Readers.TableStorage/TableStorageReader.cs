using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using ZumoCommunity.ConfigurationAPI.Infrastructure.Services;

namespace ZumoCommunity.ConfigurationAPI.Readers.TableStorage
{
	public sealed class TableStorageReader : IConfigurationReader, IConfigurationWriter
	{
		private const string DefaultPK = "";

		private readonly string _connectionString;
		private readonly string _tableName;
		private readonly CloudTable _table;

		protected CloudTable Table => _table ?? CloudStorageAccount
			.Parse(_connectionString)
			.CreateCloudTableClient()
			.GetTableReference(_tableName);

		private readonly string _keyFieldName;
		private readonly string _valueFieldName;

		public TableStorageReader(string connectionString, string tableName = "Configuration", string keyFieldName = "RowKey", string valueFieldName = "Value")
		{
			_connectionString = connectionString;
			_tableName = tableName;
			_keyFieldName = keyFieldName;
			_valueFieldName = valueFieldName;
		}

		public async Task InitializeAsync()
		{
			await Table.CreateIfNotExistsAsync();
		}

		public async Task<string> GetConfigValueAsync(string key)
		{
			var tableQuery = GetConfigurationTableQuery(key)
				.Select(new List<string> {_valueFieldName});

			var results = (await Table.ExecuteQuerySegmentedAsync(tableQuery, null)).Results;

			if (results.Any())
			{
				return results[0].Properties[_valueFieldName].StringValue;
			}
			else
			{
				return null;
			}
		}

		public async Task SetConfigValueAsync(string key, string value)
		{
			DynamicTableEntity entity;

			if (_keyFieldName.ToLowerInvariant() == "rowkey")
			{
				entity = CreateNewConfigurationEntity(key);
			}
			else
			{
				var tableQuery = GetConfigurationTableQuery(key);

				var results = (await Table.ExecuteQuerySegmentedAsync(tableQuery, null)).Results;
				if (results.Any())
				{
					entity = results[0];
				}
				else
				{
					entity = CreateNewConfigurationEntity(key);
				}
			}

			entity.Properties[_valueFieldName] = new EntityProperty(value);

			await Table.ExecuteAsync(TableOperation.InsertOrMerge(entity));
		}

		private TableQuery GetConfigurationTableQuery(string key)
		{
			return new TableQuery()
				.Take(1)
				.Where(TableQuery.CombineFilters(
					TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, DefaultPK),
					TableOperators.And,
					TableQuery.GenerateFilterCondition(_keyFieldName, QueryComparisons.Equal, key)));
		}

		private DynamicTableEntity CreateNewConfigurationEntity(string key)
		{
			return new DynamicTableEntity(DefaultPK, key);
		}
	}
}
