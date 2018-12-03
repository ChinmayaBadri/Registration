using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Microsoft.AspNet.Identity;

namespace Chinmaya.Registration.UI
{

    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CheckSessionOutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            HttpContext context = HttpContext.Current;
            if (context.Session != null && context.Session["User"] == null)
            {
                String strPathAndQuery = filterContext.HttpContext.Request.Url.PathAndQuery;
                String strUrl = filterContext.HttpContext.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

                string redirectTo = "~/Account/Login";
                string redirectOnSuccess = filterContext.HttpContext.Request.Url.ToString();
                if (!redirectOnSuccess.Contains("/Account/") && context.Request.RawUrl != "/" && !string.IsNullOrEmpty(context.Request.RawUrl) && !context.Request.RawUrl.Contains("ReturnUrl"))
                {
                    redirectTo = string.Format(redirectTo + "?returnUrl={0}", HttpUtility.UrlEncode(context.Request.RawUrl));
                    context.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    filterContext.Result = new RedirectResult(redirectTo);
                    return;
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}