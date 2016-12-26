using System;
using System.Threading.Tasks;
using ZumoCommunity.ConfigurationAPI.Infrastructure.Services;

namespace ZumoCommunity.ConfigurationAPI.Readers.Common
{
	public sealed class EnvironmentVariablesReader : IConfigurationReader, IConfigurationWriter
	{
		public async Task<string> GetConfigValueAsync(string key)
		{
			return Environment.GetEnvironmentVariable(key);
		}

		public async Task SetConfigValueAsync(string key, string value)
		{
			Environment.SetEnvironmentVariable(key, value);
		}
	}
}
