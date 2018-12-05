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
    public class EventRegistrationController : BaseController
    {
        UserService _user = new UserService();
        EventService _event = new EventService();
        // GET: EventRegistration
        public ActionResult Index()
        {
            return View();
        }

        public async Task<List<CurrentEventModel>> GetEvents(int age)
        {
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/Event/GetEventsData/" + age, true);
            return await Utility.DeserializeObject<List<CurrentEventModel>>(roleResponseMessage);
        }

        [AllowAnonymous]
        public async Task<ActionResult> ProgramEventRegistration(string[] select, string prevBtn, string nextBtn)
        {
            ProgramEventRegistrationModel programEventRegistrationModel = new ProgramEventRegistrationModel();
            programEventRegistrationModel.uFamilyMembers = await _user.GetUserFamilyMemberData(User.UserId);
            foreach (var item in programEventRegistrationModel.uFamilyMembers)
            {
                DateTime today = DateTime.Today;
                int age = today.Year - (item.DOB).Year;
                item.Events = await GetEvents(age);
				foreach (var i in item.Events)
				{
					i.ChangeAmount = (int)i.Amount;
				}

			}
            if (prevBtn != null)
            {
                return RedirectToAction("MyAccount", "Account");
            }

            else
            {
                if (nextBtn != null)
                {
                    List<ClassesConfirmModel> classesConfirm = new List<ClassesConfirmModel>();
                    if (select == null)
                    {
                        return View("ClassesConfirm", classesConfirm);
                    }
                    else
                    if (select.Length != 0)
                    {
                        List<string> selectedId = new List<string>();
                        List<List<string>> selectedEventId = new List<List<string>>();

                        for (int i = 0; i < select.Length; i++)
                        {
                            var arr = select[i].Split('_');
                            var ar1 = arr[0];
                            var ar2 = arr[1];
                            selectedId.Add(ar2);
                        }
                        List<string> userIds = selectedId.Distinct().ToList();
                        List<string> testList = new List<string>();
                        Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
                        foreach (var item in userIds)
                        {

                            for (int i = 0; i < select.Length; i++)
                            {
                                var arr = select[i].Split('_');
                                var ar1 = arr[0];
                                var ar2 = arr[1];
                                if (ar2 == item)
                                {

                                    testList.Add(ar1);

                                }
                            }
                            dict.Add(item, new List<string>(testList));
                            testList.Clear();
                        }


                        List<CurrentEventModel> currentEvents = new List<CurrentEventModel>();
                        foreach (KeyValuePair<string, List<string>> entry in dict)
                        {
                            var userData = await _user.GetUserData(entry.Key);
                            currentEvents.Clear();

                            foreach (var ev in entry.Value)
                            {
                                var eventData = await _event.GetEventData(ev);
								eventData.ChangeAmount = (int)eventData.Amount;
								currentEvents.Add(eventData);
                            }

                            ClassesConfirmModel classConfirm = new ClassesConfirmModel();
                            classConfirm.uFamilyMembers = userData;
                            classConfirm.Events = currentEvents;
                            classesConfirm.Add(classConfirm);
                        }
                        TempData["mydata"] = classesConfirm;
                        return View("ClassesConfirm", classesConfirm);
                    }

                    else return View("ProgramEventRegistration", programEventRegistrationModel);

                }
            }
            return View("ProgramEventRegistration", programEventRegistrationModel);
        }

        [AllowAnonymous]
        public ActionResult ClassesConfirm(string prevBtn, string nextBtn)
        {
            List<ClassesConfirmModel> classesConfirm = new List<ClassesConfirmModel>();
            classesConfirm = TempData["mydata"] as List<ClassesConfirmModel>;
            decimal amount = 0;
            foreach (var item in classesConfirm)
            {
                foreach (var ev in item.Events)
                {
                    amount += (decimal)ev.Amount;
                }
            }
            TempData["Amount"] = amount;
            if (prevBtn != null)
            {
                return RedirectToAction("ProgramEventRegistration");
            }

            else
            {
                if (nextBtn != null)
                {
                    if (classesConfirm == null)
                    {
                        TempData["msg"] = "<script>alert('Please select atleast one Event');</script>";
                        return RedirectToAction("ProgramEventRegistration");
                    }
                    var termCheckBox = Request.Form["termsandConditions"];
                    var dir = Request.Form["Directory"];
                    if (termCheckBox != "on")
                    {
                        TempData["msg"] = "<script>alert('Please agree to the terms and conditions');</script>";
                        return View("ClassesConfirm", classesConfirm);
                    }


                    return RedirectToAction("PaymentMethod", "Payment");
                }
            }
            return View("ClassesConfirm", classesConfirm);
        }

        [AllowAnonymous]
        public async Task<ActionResult> AddtoDirectory(string Id)
        {
            var id = User.UserId;
            HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/Event/AddtoDirectory/" + id, Id, true);
            string res = "Okay";
            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}
