using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZumoCommunity.ConfigurationAPI.Infrastructure.Services;

namespace ZumoCommunity.ConfigurationAPI.Provider
{
	public sealed class ConfigurationProvider : IConfigurationProvider
	{
		private readonly ConcurrentDictionary<string, string> _cache;

		private readonly List<IConfigurationReader> _readers;
		private readonly ReaderWriterLockSlim _readersLock;

		public ConfigurationProvider()
		{
			_cache = new ConcurrentDictionary<string, string>();

			_readersLock = new ReaderWriterLockSlim();

			_readers = new List<IConfigurationReader>();
		}

		public async Task AddConfigurationReaderAsync(IConfigurationReader reader)
		{
			_readersLock.EnterWriteLock();

			_readers.Add(reader);

			_readersLock.ExitWriteLock();
		}

		public async Task<string> GetConfigValueAsync(string key)
		{
			string value;
			if (_cache.TryGetValue(key, out value))
			{
				return value;
			}

			_readersLock.EnterReadLock();

			try
			{
				foreach (var reader in _readers)
				{
					value = await reader.GetConfigValueAsync(key);

					if (value != null)
					{
						return _cache.GetOrAdd(key, value);
					}
				}
			}
			finally
			{
				_readersLock.ExitReadLock();
			}

			return null;
		}
	}
}
