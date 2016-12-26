using System.ComponentModel.DataAnnotations;

namespace ZumoCommunity.ConfigurationAPI.API.Models
{
	public sealed class ConfigurationModel
	{
		[Required]
		public string Key { get; set; }
		[Required]
		public string Value { get; set; }
	}
}