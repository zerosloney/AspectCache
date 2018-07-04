using Aspect.Repository.Cache;
using Aspect.Web.Demo.Services;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Autofac.Integration.WebApi;
using System.Reflection;
using System.Web.Http;
using MySql.Data.MySqlClient;

namespace Aspect.Web.Demo
{
    public class AutofacStartupTask
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public static void Execute(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();
            RegisterInterface(builder);
            builder.RegisterApiControllers(Assembly.Load("Aspect.Web.Demo")).PropertiesAutowired().InstancePerRequest();//注册API
            builder.Register(c => new UseCacheInterceptor());
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        static void RegisterInterface(ContainerBuilder builder)
        {
            builder.Register<IUserService>(c => new UserService(new MySqlConnection(ConfigHelper.GetSqlConnection("db"))))
                .InstancePerRequest()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(UseCacheInterceptor));
        }
    }
}