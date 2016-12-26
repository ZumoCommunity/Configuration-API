using System.Configuration;
using System.Threading.Tasks;
using ZumoCommunity.ConfigurationAPI.Infrastructure.Services;

namespace ZumoCommunity.ConfigurationAPI.Readers.Common
{
	public sealed class AppSettingsReader : IConfigurationReader
	{
		public async Task<string> GetConfigValueAsync(string key)
		{
			return ConfigurationManager.AppSettings.Get(key);
		}
	}
}
