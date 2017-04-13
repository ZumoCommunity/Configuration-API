using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using ZumoCommunity.ConfigurationAPI.Readers.Common;

namespace ZumoCommunity.ConfigurationAPI.Tests.Readers
{
	[TestFixture]
	public class ConnectionStringsReaderTest
	{
		private ConnectionStringsReader _reader;

		[OneTimeSetUp]
		public async Task InitializeAsync()
		{
			_reader = new ConnectionStringsReader();
		}

		protected static IEnumerable<TestCaseData> GetConfigValueAsync_TestData
		{
			get
			{
				yield return new TestCaseData("key1").Returns("connection1");
				yield return new TestCaseData("key2").Returns(null);
				yield return new TestCaseData("key3").Returns("connection3");
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
