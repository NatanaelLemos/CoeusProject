using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CoeusProject.Startup))]
namespace CoeusProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
