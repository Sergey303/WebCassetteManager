using Microsoft.Owin;
using Owin;
using WebCassetteManager;

[assembly: OwinStartup(typeof(Startup))]

namespace WebCassetteManager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }

    }
}
