using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(NotificationSignalR.Startup))]
namespace NotificationSignalR
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            app.Map("/audit-notifications", map =>
            {
                var hubConfiguration = new HubConfiguration { EnableJSONP = true };//If needed
                map.RunSignalR(hubConfiguration);
            });

            // Map SignalR
            //app.MapSignalR();
        }
    }
}