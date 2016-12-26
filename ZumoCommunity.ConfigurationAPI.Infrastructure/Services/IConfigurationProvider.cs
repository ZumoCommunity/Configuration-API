using System.Threading.Tasks;

namespace ZumoCommunity.ConfigurationAPI.Infrastructure.Services
{
	public interface IConfigurationProvider : IConfigurationReader
	{
		Task AddConfigurationReaderAsync(IConfigurationReader reader);
	}
}
