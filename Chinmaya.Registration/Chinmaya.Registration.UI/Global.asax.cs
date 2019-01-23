﻿using System;
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

namespace Chinmaya.Registration.UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(MvcApplication));
        protected void Application_Start()
        {
            //logger.Info("Applicaton_Start");
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            log4net.Config.XmlConfigurator.Configure();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }
        protected void Session_End(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (authTicket != null && !authTicket.Expired)
                {
                    CustomPrincipalSerializeModel serializeModel = JsonConvert.DeserializeObject<CustomPrincipalSerializeModel>(authTicket.UserData);
                    CustomPrincipal newUser = new CustomPrincipal(authTicket.Name);
                    newUser.UserId = serializeModel.UserId;
                    newUser.FirstName = serializeModel.FirstName;
                    newUser.LastName = serializeModel.LastName;
                    newUser.roles = serializeModel.roles.ToArray();
                    HttpContext.Current.User = newUser;
                }
            }
        }

        protected void Application_Error(object sender,EventArgs e)
        {
            var unHandledExc = Server.GetLastError().GetBaseException();
            //logger.Error("Application Unhandled Exception", unHandledExc);
        }
    }
}
