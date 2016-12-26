using System.Threading.Tasks;

namespace ZumoCommunity.ConfigurationAPI.Infrastructure.Services
{
	public interface IConfigurationReader
	{
		Task<string> GetConfigValueAsync(string key);
	}
}
