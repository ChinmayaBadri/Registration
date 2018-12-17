using Chinmaya.Registration.Models;
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

		/// <summary>
		/// Gets current user info
		/// </summary>
		/// <returns> User Model</returns>
		[AllowAnonymous]
		private UserModel GetUser()
		{
			if (Session["user"] == null) Session["user"] = new UserModel();
			return (UserModel)Session["user"];
		}

		/// <summary>
		/// gets menuitems based on role
		/// </summary>
		/// <returns>partial view of menu items</returns>
		public ActionResult _MainMenu()
		{
			UserModel UserObj = GetUser();
			ViewBag.Fullname = UserObj.FirstName + " " + UserObj.LastName;
			if (UserObj != null)
			{
				var menus = GetMenusBasedOnRole(UserObj);
				ViewBag.Role = UserObj.RoleId;
				return PartialView("_MainMenu", menus);
			}
			else
			{
				return RedirectToAction("Account", "Login");
			}
		}

		/// <summary>
		/// get menus based on role
		/// </summary>
		/// <param name="UserObj"></param>
		/// <returns>list of menu items</returns>
		private List<string> GetMenusBasedOnRole(UserModel UserObj)
		{
			var menuitems = new List<string>();
			menuitems.Add("Home");
			menuitems.Add("About Us");
			menuitems.Add("Donate");
			menuitems.Add("Contact Us");
			menuitems.Add("Sign Out");

			if (UserObj.RoleId == 1)
			{
				menuitems.Add("Users");
				menuitems.Add("Current Events");
			}
			else
			{
				menuitems.Add("Program/Event Registration");
				menuitems.Add("My Account");
			}
			return menuitems;
		}
	}
}