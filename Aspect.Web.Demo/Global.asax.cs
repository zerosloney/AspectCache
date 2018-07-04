using System.Web.Http;
using Dapper;

namespace Aspect.Web.Demo
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AutofacStartupTask.Execute(GlobalConfiguration.Configuration);
            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.MySQL);
        }
    }
}
