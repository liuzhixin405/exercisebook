using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Merp.Web.UI.Startup))]
namespace Merp.Web.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

        }
    }
}
