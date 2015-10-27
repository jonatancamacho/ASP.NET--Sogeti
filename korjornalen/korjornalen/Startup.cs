using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(korjornalen.Startup))]
namespace korjornalen
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
