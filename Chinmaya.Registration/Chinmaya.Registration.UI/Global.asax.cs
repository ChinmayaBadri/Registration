using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Chinmaya.Registration.Models;
using Newtonsoft.Json;
using Chinmaya.Registration.UI.Providers;
using log4net;
using Chinmaya.UI.Filters;

namespace Chinmaya.Registration.UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure();
            GlobalFilters.Filters.Add(new ExecuteCustomErrorHandler());
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }
        protected void Session_End(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
        }

        protected void Application_PreRequestHandlerExecute(Object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            if (context.Session != null && context.Session["User"] != null)
            {
                UserModel objUser = (UserModel)context.Session["User"];
                CustomPrincipal newUser = new CustomPrincipal(objUser.Email);
                newUser.UserId = objUser.Id;
                newUser.FirstName = objUser.FirstName;
                newUser.LastName = objUser.LastName;
                HttpContext.Current.User = newUser;
            }
        }
    }
}
