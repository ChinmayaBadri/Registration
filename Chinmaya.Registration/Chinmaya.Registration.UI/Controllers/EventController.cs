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
	public class EventController : BaseController
    {
        CommonService _common = new CommonService();
		UserService _user = new UserService();

		/// <summary>
		/// Shows all events
		/// </summary>
		/// <returns> event view </returns>
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
			ViewBag.Fullname = await _user.GetUserFullName(User.Identity.Name);
			return View("Event", mainEventModel);
        }

        /// <summary>
        /// adds event
        /// </summary>
        /// <param name="data">Events model </param>
        /// <returns> Event view </returns>
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
