using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Chinmaya.Utilities;

namespace Chinmaya.UI.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            //var res = await Utility.GetAnObject<String>("/api/UserAPI/1");
            ViewBag.UserName = "res";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}