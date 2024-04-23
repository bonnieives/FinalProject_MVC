using FinalProject_MVC.DAL;
using FinalProject_MVC.DI;
using FinalProject_MVC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace FinalProject_MVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Create an instance of FinalProjectContext
            var dbContext = new FinalProjectContext();
            // Create an instance of DIContainer with FinalProjectContext as a parameter
            DIContainer container = new DIContainer(dbContext);

            // Set the resolver with the container
            DependencyResolver.SetResolver(container);
        }
    }
}
