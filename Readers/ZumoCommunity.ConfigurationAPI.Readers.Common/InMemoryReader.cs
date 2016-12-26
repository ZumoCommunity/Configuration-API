using System.Collections.Concurrent;
using System.Threading.Tasks;
using ZumoCommunity.ConfigurationAPI.Infrastructure.Services;

namespace ZumoCommunity.ConfigurationAPI.Readers.Common
{
	public sealed class InMemoryReader : IConfigurationReader, IConfigurationWriter
	{
		private readonly ConcurrentDictionary<string, string> _storage = new ConcurrentDictionary<string, string>();

		public async Task<string> GetConfigValueAsync(string key)
		{
			string value;
			if (_storage.TryGetValue(key, out value))
			{
				return value;
			}
			else
			{
				return null;
			}
		}

		public async Task SetConfigValueAsync(string key, string value)
		{
			_storage.AddOrUpdate(key, value, (k, v) => value);
		}
	}
}
