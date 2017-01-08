using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(dataShow.Startup))]
namespace dataShow
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
           
        }
    }
}
