using System.Web.Routing;
using NoSqlEvents;

namespace LiveScoreEs
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            RavenDbConfig.Initialize();
            SagaConfig.Initialize();
        }
    }
}
