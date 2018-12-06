using Chinmaya.Registration.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Chinmaya.Registration.UI.Services;
using Chinmaya.Registration.Models;

namespace Chinmaya.Registration.UI.Controllers
{
	[CustomAuthorize(Roles = "User")]
	public class PaymentController : BaseController
    {
        CommonService _common = new CommonService();
        // GET: Payment
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// takes check payment data save it to the database
        /// </summary>
        /// <param name="data">Check Payment Model</param>
        /// <param name="prevBtn"> back button name </param>
        /// <param name="nextBtn"> next button name </param>
        /// <returns> My account view or Classes confirm view </returns>
        [AllowAnonymous]
        public async Task<ActionResult> PaymentMethod(CheckPaymentModel data, string prevBtn, string nextBtn)
        {
            ViewBag.AccountType = await _common.GetAccountType();
            if (prevBtn != null)
            {
                return RedirectToAction("ClassesConfirm", "EventRegistration");
            }

            else
            {
                if (nextBtn != null)
                {
                    if (ModelState.IsValid && data.paymentType == "Check")
                    {
                        var amount = TempData["Amount"];
                        data.CreatedBy = User.UserId;
                        data.Amount = Convert.ToDecimal(amount);
                        HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/Payment/PostCheckPayment", data, true);
                        return RedirectToAction("MyAccount", "Account");

                    }

                }
            }
            return View();
        }
    }
}
