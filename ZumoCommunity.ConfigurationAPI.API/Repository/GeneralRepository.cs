using System;
using System.Threading.Tasks;
using Autofac;
using ZumoCommunity.ConfigurationAPI.Infrastructure.Services;
using ZumoCommunity.ConfigurationAPI.Readers.TableStorage;

namespace ZumoCommunity.ConfigurationAPI.API.Repository
{
	public class GeneralRepository
	{
		private static readonly Lazy<GeneralRepository> _instance = new Lazy<GeneralRepository>(() => new GeneralRepository());
		public static IContainer DependencyContainer => _instance.Value.Container;

		public IContainer Container;

		private GeneralRepository()
		{
			var configurationProvider = WebApiApplication.ConfigurationProvider;

			var builder = new ContainerBuilder();

			var task = Task.Run(() => configurationProvider.GetConfigValueAsync("ConfigurationStorage"));
			task.Wait();

			var configurationStorage = task.Result;

			var reader = new TableStorageReader(configurationStorage);
			Task.Run(() => reader.InitializeAsync()).Wait();

			builder.RegisterInstance(reader).As<IConfigurationReader>();
			builder.RegisterInstance(reader).As<IConfigurationWriter>();

			Container = builder.Build();
		}
	}
}