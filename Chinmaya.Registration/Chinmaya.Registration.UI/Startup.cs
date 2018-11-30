using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Chinmaya.Registration.UI.Startup))]
namespace Chinmaya.Registration.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
