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
    public class EventController : BaseController
    {
        CommonService _common = new CommonService();
        // GET: Event
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> Event()
        {
            MainEventModel mainEventModel = new MainEventModel();
            mainEventModel.currentEventModel = await _common.GetEvents();
			foreach (var item in mainEventModel.currentEventModel)
			{
				item.ChangeAmount = (int)item.Amount;
			}

			mainEventModel.weekday = await _common.GetWeekdayData();
            mainEventModel.frequencies = await _common.GetFrequencyData();
            mainEventModel.sessions = await _common.GetSessionData();

            return View("Event", mainEventModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> AddEvent(EventsModel data)
        {
            if (ModelState.IsValid)
            {
                data.CreatedBy = User.UserId;
                HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/Event/PostEvent", data, true);
                return RedirectToAction("Event");
            }
            return RedirectToAction("Event");
        }
    }
}
