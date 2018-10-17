using Chinmaya.Registration.UI.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chinmaya.Registration.UI.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// This methods provides to access the current user info across all the controllers
        /// which are inherited by base controller (Ex: User.UserGuid)
        /// </summary>
        protected virtual new CustomPrincipal User
        {
            get { return HttpContext.User as CustomPrincipal; }
        }
    }
}