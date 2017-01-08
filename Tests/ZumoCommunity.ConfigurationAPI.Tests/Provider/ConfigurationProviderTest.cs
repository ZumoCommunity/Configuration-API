using System.Threading.Tasks;
using NUnit.Framework;
using ZumoCommunity.ConfigurationAPI.Provider;
using ZumoCommunity.ConfigurationAPI.Readers.Common;

namespace ZumoCommunity.ConfigurationAPI.Tests.Provider
{
	[TestFixture]
	public class ConfigurationProviderTest
	{
		private ConfigurationProvider _provider;

		[SetUp]
		public async Task Initialize()
		{
			_provider = new ConfigurationProvider();
		}

		[Test]
		[TestCase("key1")]
		[TestCase("key2")]
		[TestCase("key3")]
		public async Task NullByDefault_Test(string key)
		{
			var actual = await _provider.GetConfigValueAsync(key);
			Assert.IsNull(actual);
		}

		[Test]
		[TestCase("key1", "val1", "val2")]
		[TestCase("key1", "000", "qwe")]
		[TestCase("key1", "asd", null)]
		public async Task ReturnCachedValues_Test(string key, string oldValue, string newValue)
		{
			var reader = new InMemoryReader();
			await _provider.AddConfigurationReaderAsync(reader);

			var actual = await _provider.GetConfigValueAsync(key);
			Assert.IsNull(actual);

			await reader.SetConfigValueAsync(key, oldValue);

			actual = await _provider.GetConfigValueAsync(key);
			Assert.AreEqual(oldValue, actual);

			await reader.SetConfigValueAsync(key, newValue);

			actual = await _provider.GetConfigValueAsync(key);
			Assert.AreNotEqual(newValue, actual);
			Assert.AreEqual(oldValue, actual);
		}

		[Test]
		[TestCase("key1", null, "value")]
		[TestCase("key1", "", "qwe")]
		[TestCase("key1", "     ", "real value")]
		public async Task DoNotCacheEmptyValues_Test(string key, string oldValue, string newValue)
		{
			var reader = new InMemoryReader();
			await _provider.AddConfigurationReaderAsync(reader);

			var actual = await _provider.GetConfigValueAsync(key);
			Assert.IsNull(actual);

			await reader.SetConfigValueAsync(key, oldValue);

			actual = await _provider.GetConfigValueAsync(key);
			Assert.IsNull(actual);

			await reader.SetConfigValueAsync(key, newValue);

			actual = await _provider.GetConfigValueAsync(key);
			Assert.AreNotEqual(oldValue, actual);
			Assert.AreEqual(newValue, actual);
		}

		[Test]
		[TestCase("key1", " asd ", "asd")]
		[TestCase("key1", "  123", "123")]
		[TestCase("key1", "zzz   ", "zzz")]
		public async Task TrimCachedValues_Test(string key, string value, string trimmedValue)
		{
			var reader = new InMemoryReader();
			await _provider.AddConfigurationReaderAsync(reader);

			await reader.SetConfigValueAsync(key, value);

			var actual = await _provider.GetConfigValueAsync(key);
			Assert.AreNotEqual(value, actual);
			Assert.AreEqual(trimmedValue, actual);
		}

		[Test]
		[TestCase("key1", "val1", "val2")]
		[TestCase("key1", "000", "qwe")]
		[TestCase("key1", null, "777")]
		public async Task CacheValuesOnlyAfterCall_Test(string key, string oldValue, string newValue)
		{
			var reader = new InMemoryReader();
			await _provider.AddConfigurationReaderAsync(reader);

			await reader.SetConfigValueAsync(key, oldValue);
			await reader.SetConfigValueAsync(key, newValue);

			var actual = await _provider.GetConfigValueAsync(key);
			Assert.AreNotEqual(oldValue, actual);
			Assert.AreEqual(newValue, actual);
		}

		[Test]
		[TestCase("key1", "oldValue", "newValue")]
		[TestCase("key1", "000", "qwe")]
		[TestCase("key1", "000", "777")]
		public async Task ReturnValuesFromFirstProviderFirst_Test(string key, string oldValue, string newValue)
		{
			var firstReader = new InMemoryReader();
			await firstReader.SetConfigValueAsync(key, newValue);

			var secondReader = new InMemoryReader();
			await secondReader.SetConfigValueAsync(key, oldValue);

			await _provider.AddConfigurationReaderAsync(firstReader);
			await _provider.AddConfigurationReaderAsync(secondReader);

			var actual = await _provider.GetConfigValueAsync(key);
			Assert.AreNotEqual(oldValue, actual);
			Assert.AreEqual(newValue, actual);
		}

		[Test]
		[TestCase("key1", "some value", "connection1")]
		[TestCase("key2", "test", "value2")]
		[TestCase("key3", "000", "connection3")]
		[TestCase("key4", "hello world", "hello world")]
		public async Task CacheValuesFromProviders_Test(string key, string value, string expected)
		{
			var firstReader = new ConnectionStringsReader();
			var secondReader = new AppSettingsReader();
			var thirdReader = new InMemoryReader();

			await thirdReader.SetConfigValueAsync(key, value);

			await _provider.AddConfigurationReaderAsync(firstReader);
			await _provider.AddConfigurationReaderAsync(secondReader);
			await _provider.AddConfigurationReaderAsync(thirdReader);

			var actual = await _provider.GetConfigValueAsync(key);
			Assert.AreEqual(expected, actual);
		}
	}
}
