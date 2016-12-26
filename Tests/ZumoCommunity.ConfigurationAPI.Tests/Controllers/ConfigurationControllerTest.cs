using System.Threading.Tasks;
using Autofac;
using NUnit.Framework;
using ZumoCommunity.ConfigurationAPI.API.Controllers;
using ZumoCommunity.ConfigurationAPI.API.Models;
using ZumoCommunity.ConfigurationAPI.Infrastructure.Services;
using ZumoCommunity.ConfigurationAPI.Readers.Common;

namespace ZumoCommunity.ConfigurationAPI.Tests.Controllers
{
	[TestFixture]
	public class ConfigurationControllerTest
	{
		private IConfigurationReader _reader;
		private IConfigurationWriter _writer;

		private ConfigurationController _controller;

		[SetUp]
		public async Task Initialize()
		{
			var builder = new ContainerBuilder();

			var reader = new InMemoryReader();
			_reader = reader;
			_writer = reader;

			builder.RegisterInstance(_reader).As<IConfigurationReader>();
			builder.RegisterInstance(_writer).As<IConfigurationWriter>();

			var container = builder.Build();

			_controller = new ConfigurationController(container);
		}

		[Test]
		[TestCase("key", "value")]
		[TestCase("ConnectionString", "UseDevelopmentStorage=true;")]
		public async Task GetAsync_Test(string key, string value)
		{
			var actual = await _controller.GetAsync(key);
			Assert.IsNull(actual);

			await _controller.SetAsync(new ConfigurationModel {Key = key, Value = value});

			actual = await _controller.GetAsync(key);
			Assert.AreEqual(value, actual);
		}

		[Test]
		[TestCase("key", "value", "newValue")]
		[TestCase("ConnectionString", "UseDevelopmentStorage=true;", "bla-bla")]
		public async Task SetAsync_Test(string key, string oldValue, string newValue)
		{
			var actual = await _controller.GetAsync(key);
			Assert.IsNull(actual);

			await _controller.SetAsync(new ConfigurationModel { Key = key, Value = oldValue });

			actual = await _controller.GetAsync(key);
			Assert.AreEqual(oldValue, actual);

			await _controller.SetAsync(new ConfigurationModel { Key = key, Value = newValue });

			actual = await _controller.GetAsync(key);
			Assert.AreEqual(newValue, actual);
		}
	}
}
