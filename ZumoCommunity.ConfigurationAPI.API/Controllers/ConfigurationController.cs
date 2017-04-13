using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using ZumoCommunity.ConfigurationAPI.API.Models;
using ZumoCommunity.ConfigurationAPI.API.Repository;
using ZumoCommunity.ConfigurationAPI.Infrastructure.Services;

namespace ZumoCommunity.ConfigurationAPI.API.Controllers
{
	[RoutePrefix("api/v1/configuration")]
	public class ConfigurationController : ApiController
	{
		private IConfigurationReader _configurationReader;
		private IConfigurationWriter _configurationWriter;

		public ConfigurationController()
		{
			Initialize(GeneralRepository.DependencyContainer);
		}

		public ConfigurationController(IContainer container)
		{
			Initialize(container ?? GeneralRepository.DependencyContainer);
		}

		private void Initialize(IContainer container)
		{
			_configurationReader = container.Resolve<IConfigurationReader>();
			_configurationWriter = container.Resolve<IConfigurationWriter>();
		}

		[HttpGet]
		[Route("{key}")]
		public async Task<string> GetAsync(string key)
		{
			return await _configurationReader.GetConfigValueAsync(key);
		}

		[HttpPost]
		[Route("")]
		public async Task SetAsync([FromBody]ConfigurationModel model)
		{
			await _configurationWriter.SetConfigValueAsync(model.Key, model.Value);
		}
	}
}
