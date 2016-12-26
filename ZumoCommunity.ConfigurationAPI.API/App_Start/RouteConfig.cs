using System.Web.Http;
using System.Web.Routing;
using Swashbuckle.Application;

namespace ZumoCommunity.ConfigurationAPI.API
{
	public static class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.MapHttpRoute(
				name: "swagger_root",
				routeTemplate: "",
				defaults: null,
				constraints: null,
				handler: new RedirectHandler((message => message.RequestUri.ToString()), "swagger"));
		}
	}
}