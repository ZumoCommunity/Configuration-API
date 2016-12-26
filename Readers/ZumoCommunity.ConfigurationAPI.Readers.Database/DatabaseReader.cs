using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using ZumoCommunity.ConfigurationAPI.Infrastructure.Services;

namespace ZumoCommunity.ConfigurationAPI.Readers.Database
{
	public sealed class DatabaseReader : IConfigurationReader, IConfigurationWriter
	{
		private readonly string _connectionString;
		private readonly string _tableName;
		private readonly string _keyFieldName;
		private readonly string _valueFieldName;

		public DatabaseReader(string connectionString, string tableName = "Configuration", string keyFieldName = "Key", string valueFieldName = "Value")
		{
			_connectionString = connectionString;
			_tableName = tableName;
			_keyFieldName = keyFieldName;
			_valueFieldName = valueFieldName;
		}

		public async Task InitializeAsync()
		{
			using (var sqlConnection = new SqlConnection(_connectionString))
			{
				await sqlConnection.OpenAsync();

				var sqlCommand = GetCreateTableCommand();

				await sqlCommand.ExecuteNonQueryAsync();
			}
		}

		public async Task<string> GetConfigValueAsync(string key)
		{
			using (var sqlConnection = new SqlConnection(_connectionString))
			{
				await sqlConnection.OpenAsync();

				var sqlCommand = GetValueByKeyCommand(key);

				var result = await sqlCommand.ExecuteScalarAsync();

				return result as string;
			}
		}

		public async Task SetConfigValueAsync(string key, string value)
		{
			using (var sqlConnection = new SqlConnection(_connectionString))
			{
				await sqlConnection.OpenAsync();

				var sqlCommand = GetRowByKeyCommand(key);

				var result = await sqlCommand.ExecuteReaderAsync();

				sqlCommand = result.HasRows ?
					GetUpdateCommand(key, value) :
					GetInsertCommand(key, value);

				await sqlCommand.ExecuteNonQueryAsync();
			}
		}

		private SqlCommand GetValueByKeyCommand(string key)
		{
			var sqlCommand = new SqlCommand();

			sqlCommand.CommandText = string.Format("SELECT {2} FROM {0} WHERE {1} = @Key", _tableName, _keyFieldName, _valueFieldName);
			sqlCommand.Parameters.AddWithValue("Key", key);

			return sqlCommand;
		}

		private SqlCommand GetRowByKeyCommand(string key)
		{
			var sqlCommand = new SqlCommand();

			sqlCommand.CommandText = string.Format("SELECT * FROM {0} WHERE {1} = @Key", _tableName, _keyFieldName);
			sqlCommand.Parameters.AddWithValue("Key", key);

			return sqlCommand;
		}

		private SqlCommand GetInsertCommand(string key, string value)
		{
			var sqlCommand = new SqlCommand();

			sqlCommand.CommandText = string.Format("INSERT INTO {0} ({1}, {2}) VALUES (@Key, @Value)", _tableName, _keyFieldName, _valueFieldName);
			sqlCommand.Parameters.AddWithValue("Key", key);
			sqlCommand.Parameters.AddWithValue("Value", value);

			return sqlCommand;
		}

		private SqlCommand GetUpdateCommand(string key, string value)
		{
			var sqlCommand = new SqlCommand();

			sqlCommand.CommandText = string.Format("UPDATE {0} SET {2} = @Value WHERE {1} = @Key", _tableName, _keyFieldName, _valueFieldName);
			sqlCommand.Parameters.AddWithValue("Key", key);
			sqlCommand.Parameters.AddWithValue("Value", value);

			return sqlCommand;
		}

		private SqlCommand GetCreateTableCommand()
		{
			var text = new StringBuilder();
			text.Append("if not exists ");
			text.AppendFormat("(SELECT * FROM sysobjects WHERE name='{0}' and xtype = 'U') ", _tableName);
			text.AppendFormat("CREATE TABLE '{0}' (", _tableName);
			text.AppendFormat("[{0}] nvarchar(max) not null,", _keyFieldName);
			text.AppendFormat("[{0}] nvarchar(max) not null)", _valueFieldName);

			var sqlCommand = new SqlCommand();

			sqlCommand.CommandText = text.ToString();

			return sqlCommand;
		}
	}
}
