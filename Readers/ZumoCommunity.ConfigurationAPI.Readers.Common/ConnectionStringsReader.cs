using System.Configuration;
using System.Threading.Tasks;
using ZumoCommunity.ConfigurationAPI.Infrastructure.Services;

namespace ZumoCommunity.ConfigurationAPI.Readers.Common
{
	public sealed class ConnectionStringsReader : IConfigurationReader
	{
		public async Task<string> GetConfigValueAsync(string key)
		{
			var connectionString = ConfigurationManager.ConnectionStrings[key];

			if (connectionString == null)
			{
				return null;
			}
			else
			{
				return connectionString.ConnectionString;
			}
		}
	}
}
