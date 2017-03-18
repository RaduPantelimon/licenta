using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ResourceApplicationTool.Startup))]
namespace ResourceApplicationTool
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
