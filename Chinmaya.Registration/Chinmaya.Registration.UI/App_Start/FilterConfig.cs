using Chinmaya.Registration.UI.Providers;
using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace Chinmaya.Registration.UI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new NoCacheAttribute());
        }
    }
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            string returnUrl = filterContext.HttpContext.Request.RawUrl;
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                if (!String.IsNullOrEmpty(Roles))
                {
                    if (SessionVar.LoginUser == null)
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login", returnUrl = returnUrl }));
                    }
                    else
                    {
                        string[] arrRoles = Roles.Split(',');
                        if (Array.IndexOf(arrRoles, SessionVar.LoginUser.RoleName) <= -1)
                        {
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "NotAuthorized", returnUrl = returnUrl }));
                        }
                    }
                }
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "NotAuthorized", returnUrl = returnUrl }));
            }
        }
    }
}
