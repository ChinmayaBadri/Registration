using Chinmaya.Registration.Models;
using Chinmaya.Registration.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Chinmaya.Registration.UI.Services;

namespace Chinmaya.Registration.UI.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
		AccountService _account = new AccountService();

		// GET: Admin
		public async Task<ActionResult> Index()
        {
			ViewBag.Fullname = await _account.GetUserFullName(User.Identity.Name);
			return View();
        }

        public async Task<ActionResult> GetAllUsers()
        {
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/User/GetAllUsers", true);
            var users = await Utility.DeserializeObject<List<UserInfoModel>>(roleResponseMessage);
            return Json(new { data = users }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAllFamilyMembers(string Id)
        {
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/User/GetAllFamilyMembers/" + Id, true);
            var familyMembers = await Utility.DeserializeObject<List<UFamilyMember>>(roleResponseMessage);
            return Json(new { data = familyMembers }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ChangeAccountType(string Id)
        {
            HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/Account/ChangeAccountType/" + Id, Id, true);
            return RedirectToAction("Index");
        }
    }
}
