using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using ZumoCommunity.ConfigurationAPI.Readers.Common;

namespace ZumoCommunity.ConfigurationAPI.Tests.Readers
{
	[TestFixture]
	public class AppSettingsReaderTest
	{
		private AppSettingsReader _reader;

		[OneTimeSetUp]
		public async Task InitializeAsync()
		{
			_reader = new AppSettingsReader();
		}

		protected static IEnumerable<TestCaseData> GetConfigValueAsync_TestData
		{
			get
			{
				yield return new TestCaseData("key1").Returns("value1");
				yield return new TestCaseData("key2").Returns("value2");
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
