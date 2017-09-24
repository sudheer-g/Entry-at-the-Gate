using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
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
            //ModelBinders.Binders.Add(typeof(DateTime), new CustomDateModelBinder());

            Dictionary<string, int> guestEscortMap = new Dictionary<string, int>();
            Queue<ApplicationUser> escortQueue = new Queue<ApplicationUser>();
            Application["guestEscortMap"] = guestEscortMap;
            Application["escortQueue"] = escortQueue;
            Application["firstAssignment"] = 0;
        }
    }
    //public class CustomDateModelBinder : DefaultModelBinder
    //{
    //    public override object BindModel
    //    (ControllerContext controllerContext, ModelBindingContext bindingContext)
    //    {
    //        var displayFormat = bindingContext.ModelMetadata.DisplayFormatString;
    //        var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

    //        if (!string.IsNullOrEmpty(displayFormat) && value != null)
    //        {
    //            DateTime date;
    //            displayFormat = displayFormat.Replace
    //            ("{0:", string.Empty).Replace("}", string.Empty);
    //            if (DateTime.TryParseExact(value.AttemptedValue, displayFormat,
    //            CultureInfo.GetCultureInfo("en-IN"), DateTimeStyles.None, out date))
    //            {
    //                return date;
    //            }
    //            else
    //            {
    //                bindingContext.ModelState.AddModelError(
    //                    bindingContext.ModelName,
    //                    string.Format("{0} is an invalid date format", value.AttemptedValue)
    //                );
    //            }
    //        }

    //        return base.BindModel(controllerContext, bindingContext);
    //    }
    //}
}
