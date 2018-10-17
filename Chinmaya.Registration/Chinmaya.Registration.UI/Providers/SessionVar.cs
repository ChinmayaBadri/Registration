using Chinmaya.Registration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chinmaya.Registration.UI.Providers
{
    public class SessionVar
    {
        public static UserModel LoginUser
        {
            get
            {
                return HttpContext.Current.Session["User"] as UserModel;
            }
            set
            {
                HttpContext.Current.Session["User"] = value;
            }
        }
    }
}