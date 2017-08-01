using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Stack_Exchange_Voting_Utility.Models;

namespace Stack_Exchange_Voting_Utility
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e) =>
            LogError(Server.GetLastError(), Request);

        internal static void LogError(Exception e, HttpRequest request)
        {
            using (var db = new ApplicationDbContext())
            {
                db.ErrorLog.Add(new ErrorLog(e, request));
                db.SaveChanges();
            }
        }

        internal static void LogError(Exception e, HttpRequestBase request)
        {
            using (var db = new ApplicationDbContext())
            {
                db.ErrorLog.Add(new ErrorLog(e, request));
                db.SaveChanges();
            }
        }
    }
}
