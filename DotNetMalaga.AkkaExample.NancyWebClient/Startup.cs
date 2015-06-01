using Microsoft.Owin;
using Owin;

namespace DotNetMalaga.AkkaExample.NancyWebClient
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //app.MapSignalR();
            //app.Map("/signalr", map =>
            //{
            //    var hubConfiguration = new HubConfiguration();
            //    map.RunSignalR(hubConfiguration);
            //}).RunSignalR();

            //var configuration = new HubConfiguration { EnableDetailedErrors = true, EnableJSONP = true };
            //app.MapSignalR("/signalr", configuration);
            //app.UseNancy();
        }
    }
}
