using Ninject;
using Ninject.Web.Common.WebHost;
using RLCodeTest.DAL.Interfaces;
using RLCodeTest.DAL.Repositories;
using RLCodeTest.Services.Interfaces;
using RLCodeTest.Services.Services;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace RLCodeTest
{
    // Inherit from NinjectHttpApplication so we can implement dependency injection
    public class MvcApplication : NinjectHttpApplication
    {
        protected override void OnApplicationStarted()
        {
            base.OnApplicationStarted();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected override IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            RegisterServices(kernel);
            return kernel;
        }

        private void RegisterServices(IKernel kernel)
        {
            // Register MaturityDataService
            kernel.Bind<IMaturityDataService>().To<MaturityDataService>();

            // Register CSVMaturityDataRepository - this could easily be swapped out for an alternative data repository 
            kernel.Bind<IMaturityDataRepository>().To<CSVMaturityDataRepository>();
        }
    }
}
