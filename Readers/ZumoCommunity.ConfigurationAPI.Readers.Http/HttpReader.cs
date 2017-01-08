using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using ZumoCommunity.ConfigurationAPI.Infrastructure.Services;

namespace ZumoCommunity.ConfigurationAPI.Readers.Http
{
	public sealed class HttpReader : IConfigurationReader, IConfigurationWriter
	{
		private readonly string _url;
		private readonly string _keyName;
		private readonly string _valueName;

		public HttpReader(string baseUrl, string apiUrl = "api/configuration", string keyName = "key", string valueName = "value")
		{
			var urlBuilder = new StringBuilder();
			urlBuilder.Append(baseUrl.Trim().TrimEnd('/'));
			urlBuilder.Append('/');
			urlBuilder.Append(apiUrl.Trim(' ', '/'));

			_url = urlBuilder.ToString();
			_keyName = keyName;
			_valueName = valueName;
		}

		public async Task<string> GetConfigValueAsync(string key)
		{
			using (var client = new HttpClient())
			{
				return await client.GetStringAsync($"{_url}/{key}");
			}
		}

		public async Task SetConfigValueAsync(string key, string value)
		{
			using (var client = new HttpClient())
			{
				var values = new Dictionary<string, string>
				{
					{ _keyName, key },
					{ _valueName, value }
				};

				var content = new FormUrlEncodedContent(values);

				var response = await client.PostAsync(_url, content);

				if (!response.IsSuccessStatusCode)
				{
					throw new HttpResponseException(HttpStatusCode.InternalServerError);
				}
			}
		}
	}
}
