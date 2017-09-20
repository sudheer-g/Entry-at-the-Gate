using IdentitySample.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace IdentitySample
{
    // Note: For instructions on enabling IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=301868
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Dictionary<string, int> guestEscortMap = new Dictionary<string, int>();
            Queue<ApplicationUser> escortQueue = new Queue<ApplicationUser>();
            Application["guestEscortMap"] = guestEscortMap;
            Application["escortQueue"] = escortQueue;
            Application["firstAssignment"] = 0;
        }
    }
}
