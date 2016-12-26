using System.Threading.Tasks;

namespace ZumoCommunity.ConfigurationAPI.Infrastructure.Services
{
	public interface IConfigurationWriter
	{
		Task SetConfigValueAsync(string key, string value);
	}
}
