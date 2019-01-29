using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chinmaya.Registration.UI;
using Chinmaya.Registration.UI.Providers;

namespace Chinmaya.UI.Filters
{
    public class ExecuteCustomErrorHandler : ActionFilterAttribute, IExceptionFilter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MvcApplication));

        public void OnException(ExceptionContext filterContext)
        {
            string AuthenticatedUserName = string.Empty;
            if (SessionVar.LoginUser != null) AuthenticatedUserName = SessionVar.LoginUser.FirstName + " " + SessionVar.LoginUser.LastName;

            Log.Error("Unhandled exception logged in Application." + Environment.NewLine +
                "User : " + AuthenticatedUserName + Environment.NewLine +
                "Page : " + HttpContext.Current.Request.Url.AbsoluteUri, filterContext.Exception);

        }
    }
}