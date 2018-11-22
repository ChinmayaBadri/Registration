using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Chinmaya.Registration.UI
{

    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CheckSessionOutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            var User = session["User"];
            if (((User == null) && (!session.IsNewSession)))
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    //filterContext.HttpContext.Response.StatusCode = 403;
                    //filterContext.HttpContext.Response.StatusDescription = "Session Timeout";
                    //filterContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;
                    //filterContext.HttpContext.Response.End();                
                }
            }
        }
    }
}