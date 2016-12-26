using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using ZumoCommunity.ConfigurationAPI.Readers.Common;

namespace ZumoCommunity.ConfigurationAPI.Tests.Readers
{
	[TestFixture]
	public class InMemoryWriterTest
	{
		private InMemoryReader _reader;

		[SetUp]
		public void Initialize()
		{
			_reader = new InMemoryReader();
		}

		protected static IEnumerable<TestCaseData> SetNewConfigValueAsync_TestData
		{
			get
			{
				yield return new TestCaseData("key1", "123");
				yield return new TestCaseData("key2", "qwe");
			}
		}

		[Test]
		[TestCaseSource(nameof(SetNewConfigValueAsync_TestData))]
		public async Task SetNewConfigValueAsync_Test(string key, string value)
		{
			var actual = await _reader.GetConfigValueAsync(key);
			Assert.IsNull(actual);

			await _reader.SetConfigValueAsync(key, value);

			actual = await _reader.GetConfigValueAsync(key);
			Assert.AreEqual(value, actual);
		}

		protected static IEnumerable<TestCaseData> OverwrteConfigValueAsync_TestData
		{
			get
			{
				yield return new TestCaseData("key1", "123", "asd");
				yield return new TestCaseData("key2", "qwe", "000");
			}
		}

		[Test]
		[TestCaseSource(nameof(OverwrteConfigValueAsync_TestData))]
		public async Task OverwrteConfigValueAsync_Test(string key, string value1, string value2)
		{
			var actual = await _reader.GetConfigValueAsync(key);
			Assert.IsNull(actual);

			await _reader.SetConfigValueAsync(key, value1);

			actual = await _reader.GetConfigValueAsync(key);
			Assert.AreEqual(value1, actual);

			await _reader.SetConfigValueAsync(key, value2);

			actual = await _reader.GetConfigValueAsync(key);
			Assert.AreEqual(value2, actual);
		}

	}
}
