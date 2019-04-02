using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Extensions.Configuration;

namespace Koenig.Maestro.Host
{
    public class MvcApplication : System.Web.HttpApplication
    {

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected void Application_Start()
        {
            logger.Debug("starting...");
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            IConfigurationRoot configRoot = new ConfigurationBuilder().SetBasePath(Server.MapPath("~"))
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true).Build();
            Maestro.Operation.MaestroApplication.ConfigRoot = configRoot;

        }
    }
}
