using System.Net;
using Microsoft.Owin;
using Owin;
[assembly: OwinStartup(typeof(Web.Startup))]
namespace Web
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			AllowBetterCrypto();

			app.MapSignalR();
		}

		private static void AllowBetterCrypto()
		{
			// by default TLS 1.1 and 1.2 are not enabled
			ServicePointManager.SecurityProtocol =
				SecurityProtocolType.Ssl3 |
				SecurityProtocolType.Tls |
				SecurityProtocolType.Tls11 |
				SecurityProtocolType.Tls12;
		}
	}
}