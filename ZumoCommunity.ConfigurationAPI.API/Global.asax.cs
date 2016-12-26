using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;
using ZumoCommunity.ConfigurationAPI.Infrastructure.Services;
using ZumoCommunity.ConfigurationAPI.Provider;
using ZumoCommunity.ConfigurationAPI.Readers.Common;

namespace ZumoCommunity.ConfigurationAPI.API
{
	public class WebApiApplication : System.Web.HttpApplication
	{
		public static IConfigurationProvider ConfigurationProvider;

		protected void Application_Start()
		{
			GlobalConfiguration.Configure(WebApiConfig.Register);
			RouteConfig.RegisterRoutes(RouteTable.Routes);

			InitializeConfigurationProvider();
		}

		public void InitializeConfigurationProvider(IConfigurationProvider configurationProvider = null)
		{
			ConfigurationProvider = configurationProvider ?? new ConfigurationProvider();
			var tasks = new[]
			{
				Task.Run(() => ConfigurationProvider.AddConfigurationReaderAsync(new EnvironmentVariablesReader())),
				Task.Run(() => ConfigurationProvider.AddConfigurationReaderAsync(new ConnectionStringsReader())),
				Task.Run(() => ConfigurationProvider.AddConfigurationReaderAsync(new AppSettingsReader()))
			};
			Task.WaitAll(tasks);
		}
	}
}
