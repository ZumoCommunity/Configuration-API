using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using ZumoCommunity.ConfigurationAPI.Readers.Common;

namespace ZumoCommunity.ConfigurationAPI.Tests.Readers
{
	[TestFixture]
	public class EnvironmentVariablesReaderTest
	{
		private EnvironmentVariablesReader _reader;

		[OneTimeSetUp]
		public async Task Initialize()
		{
			_reader = new EnvironmentVariablesReader();

			await _reader.SetConfigValueAsync("key1", "123");
			await _reader.SetConfigValueAsync("key2", "qwe");
		}

		[OneTimeTearDown]
		public async Task Cleanup()
		{
			_reader = new EnvironmentVariablesReader();

			await _reader.SetConfigValueAsync("key1", null);
			await _reader.SetConfigValueAsync("key2", null);
		}

		protected static IEnumerable<TestCaseData> GetConfigValueAsync_TestData
		{
			get
			{
				yield return new TestCaseData("key1").Returns("123");
				yield return new TestCaseData("key2").Returns("qwe");
				yield return new TestCaseData("key3").Returns(null);
			}
		}

		[Test]
		[TestCaseSource(nameof(GetConfigValueAsync_TestData))]
		public async Task<string> GetConfigValueAsync_Test(string key)
		{
			return await _reader.GetConfigValueAsync(key);
		}
	}
}
