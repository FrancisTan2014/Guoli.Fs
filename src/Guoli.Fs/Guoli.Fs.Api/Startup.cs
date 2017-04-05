using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Guoli.Fs.Api.Startup))]
namespace Guoli.Fs.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
