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
		EventService _event = new EventService();

		/// <summary>
		/// Shows all events
		/// </summary>
		/// <returns> event view </returns>
		[AllowAnonymous]
        public async Task<ActionResult> Event(ToastModel tm = null)
        {
			ViewBag.msg = TempData["msg"] as string;
			if (!string.IsNullOrEmpty(tm.Message))
			{
				ViewBag.tm = tm;
			}
			MainEventModel mainEventModel = new MainEventModel();
            mainEventModel.currentEventModel = await _common.GetEvents();
			foreach (var item in mainEventModel.currentEventModel)
			{
				if (item.Amount != null)
					item.ChangeAmount = (int)item.Amount;
				else
					item.ChangeAmount = 0;
			}
			
			mainEventModel.events.weekday = await _common.GetWeekdayData();
            mainEventModel.events.frequencies = await _common.GetFrequencyData();
            mainEventModel.events.sessions = await _common.GetSessionData();
			ViewBag.CountryList = await _common.GetCountryData();
			return View("Event", mainEventModel);
        }

		/// <summary>
		/// gets Event partial view with data binding
		/// </summary>
		/// <returns> Event partial view </returns>
		[HttpGet]
		[AllowAnonymous]
		public async Task<PartialViewResult> RefreshEventPartialView()
		{
			EventsModel em = new EventsModel();
			em.weekday = await _common.GetWeekdayData();
			em.frequencies = await _common.GetFrequencyData();
			em.sessions = await _common.GetSessionData();
			em.StartDate = DateTime.Now;
			em.EndDate = DateTime.Now;
			em.StartTime = TimeSpan.FromTicks(DateTime.Now.Ticks);
			em.EndTime = TimeSpan.FromTicks(DateTime.Now.Ticks);
			em.HolidayDate = DateTime.Now;
			return PartialView("_AddEvent", em);
		}

		/// <summary>
		/// adds event
		/// </summary>
		/// <param name="data">Events model </param>
		/// <returns> Event view </returns>
		[HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> AddEvent(EventsModel data, string nextBtn)
        {
			ToastModel tm = new ToastModel();
			data.weekday = await _common.GetWeekdayData();
			data.frequencies = await _common.GetFrequencyData();
			data.sessions = await _common.GetSessionData();
			if (nextBtn != null)
			{
				if (ModelState.IsValid)
				{
					data.CreatedBy = User.UserId;
					data.CreatedDate = DateTime.Now;
					data.UpdatedBy = User.UserId;
					HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/Event/PostEvent", data, true);
					var msg = await Utility.DeserializeObject<string>(userResponseMessage);
					if (msg == "Event successfully added" || msg == "Event successfully edited")
					{
						tm.IsSuccess = true;
						tm.Message = msg;
					}
					else
					{
						tm.IsSuccess = false;
						tm.Message = msg;
					}
					return Json(tm);
				}
				//else
				//{
				//	tm.IsSuccess = false;
				//	tm.Message = "Event not created";
				//}
				
			}
            return RedirectToAction("Event", "Event");
        }

		/// <summary>
		/// gets event details
		/// </summary>
		/// <param name="Id"> event id </param>
		/// <returns> event partial view </returns>
		public async Task<PartialViewResult> EditEvent(string Id)
		{
			EventsModel em = await _event.GetEventDetails(Id);
			//if (em.StartDate == Convert.ToDateTime(1/1/0001) || em.EndDate == Convert.ToDateTime(1/1/0001))
			//{
			//	em.StartDate = DateTime.Now;
			//	em.EndDate = DateTime.Now;
			//}
			em.Id = Id;
			em.weekday = await _common.GetWeekdayData();
			em.frequencies = await _common.GetFrequencyData();
			em.sessions = await _common.GetSessionData();

			return PartialView("_AddEvent", em);
		}

		/// <summary>
		/// deletes event details
		/// </summary>
		/// <param name="Id"> event id </param>
		/// <returns> event view </returns>
		public async Task<string> DeleteEvent(string Id)
		{
			HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/Event/DeleteEvent/" + Id, Id, true);
			var msg = await Utility.DeserializeObject<string>(userResponseMessage);
			return msg;
		}
	}
}
