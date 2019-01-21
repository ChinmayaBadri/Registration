using Chinmaya.Registration.Models;
using Chinmaya.Registration.UI.Services;
using Chinmaya.Registration.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Chinmaya.Registration.UI.Controllers
{
    [CustomAuthorize(Roles = "Admin, User")]
    public class CommonController : BaseController
    {
        AccountService _account = new AccountService();
        UserService _user = new UserService();
        CommonService _common = new CommonService();

        /// <summary>
        /// Changes user account password
        /// </summary>
        /// <param name="Info"> Update Password Model</param>
        /// <param name="nextBtn"> </param>
        /// <returns> My Account view </returns>
		[HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ChangePassword(UpdatePasswordModel Info)
        {
            ToastModel tm = new ToastModel();
			string role = await _common.GetUserRoleName(User.Identity.Name);
            if (ModelState.IsValid)
            {
                UpdatePasswordModel passwordModel = new UpdatePasswordModel();
                passwordModel.Email = User.Identity.Name;
                EncryptDecrypt objEncryptDecrypt = new EncryptDecrypt();
                passwordModel.OldPassword = objEncryptDecrypt.Encrypt(Info.OldPassword, WebConfigurationManager.AppSettings["ServiceAccountPassword"]);
                EncryptDecrypt objEncryptDecrypt1 = new EncryptDecrypt();
                passwordModel.NewPassword = objEncryptDecrypt.Encrypt(Info.NewPassword, WebConfigurationManager.AppSettings["ServiceAccountPassword"]);
                HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/User/UpdatePassword", passwordModel, true);
                bool status = await Utility.DeserializeObject<bool>(userResponseMessage);
                if (status == true)
                {
                    tm.IsSuccess = true;
                    tm.Message = "Password updated successfully";
                }
                else
                {
                    tm.IsSuccess = false;
                    tm.Message = "Failed to update Password"; //Old password Does not match
                }
                return Json(tm);
            }
			return PartialView("_ChangePassword");
		}

        /// <summary>
        /// Changes user phone no.
        /// </summary>
        /// <param name="Info"> Update phone model </param>
        /// <param name="nextBtn"></param>
        /// <returns> My Account view </returns>
		[HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ChangePhone(UpdatePhone Info)
        {
            ToastModel tm = new ToastModel();
			string role = await _common.GetUserRoleName(User.Identity.Name);
            if (ModelState.IsValid)
            {
                UpdatePhone phoneModel = new UpdatePhone();
                phoneModel.Email = User.Identity.Name;
                phoneModel.OldPhone = Info.OldPhone;
                HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/User/UpdatePhone", phoneModel, true);
                tm.IsSuccess = true;
                tm.Message = "Phone updated successfully";
                return Json(tm);
            }
			return RedirectToAction("EditPhoneNumber", "Common");
		}

        /// <summary>
        /// Changes User account email
        /// </summary>
        /// <param name="em">Update Email model </param>
        /// <param name="nextBtn"></param>
        /// <returns> My Account view </returns>
		[HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ChangeEmail(UpdateEmail em)
        {
            ToastModel tm = new ToastModel();
			string role = await _common.GetUserRoleName(User.Identity.Name);
            if (ModelState.IsValid)
            {
                string uId = await _account.GetUserIdByEmail(User.Identity.Name);
                bool isEmailExists = await _account.CheckIsEmailExists(em.Email);
                if (isEmailExists)
                {
                    tm.IsSuccess = false;
                    tm.Message = "Email already exists";
                    return Json(tm);
                }
                else
                {
                    UpdateEmail emailModel = new UpdateEmail();
                    emailModel.userId = uId;
                    emailModel.Email = em.Email;
                    HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/User/UpdateEmailAddress", emailModel, true);
                    tm.IsSuccess = true;
                    tm.Message = "Email updated successfully";
                    return Json(tm);
                }
            }
			return RedirectToAction("EditEmail", "Common");
		}

        /// <summary>
        /// Changes user address
        /// </summary>
        /// <param name="Info"> Contact Details model </param>
        /// <param name="nextBtn"></param>
        /// <returns> My Account view </returns>
		[HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ChangeAddress(ContactDetails Info)
        {
            ToastModel tm = new ToastModel();
			string role = await _common.GetUserRoleName(User.Identity.Name);
			if (ModelState.IsValid)
			{
				ContactDetails cntd = new ContactDetails();
				cntd.Email = User.Identity.Name;
				cntd.Address = Info.Address;
				cntd.City = Info.City;
				cntd.State = Info.State;
				cntd.Country = Info.Country;
				cntd.ZipCode = Info.ZipCode;
				cntd.HomePhone = Info.HomePhone;
				HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/User/UpdateAddress", cntd, true);
				tm.IsSuccess = true;
				tm.Message = "Address updated successfully";
				return Json(tm);
			}
			else
				return RedirectToAction("EditAddress", "Common");
		}

        /// <summary>
        /// gets user email
        /// </summary>
        /// <returns> Change Email partial view </returns>
		[HttpGet]
        [AllowAnonymous]
        public PartialViewResult EditEmail()
        {
            UpdateEmail em = new UpdateEmail();
            em.Email = User.Identity.Name;
            return PartialView("_ChangeEmail", em);
        }

        /// <summary>
        /// gets user address
        /// </summary>
        /// <returns> Change Address partial view </returns>
		[HttpGet]
        [AllowAnonymous]
        public async Task<PartialViewResult> EditAddress()
        {
            ContactDetails cd = await _user.getAddress(User.Identity.Name);
			cd.Email = User.Identity.Name;
			ViewBag.CountryList = await _common.GetCountryData();
            ViewBag.SelectedCountry = cd.Country;
            ViewBag.SelectedState = cd.State;
            return PartialView("_ChangeAddress", cd);
        }

        /// <summary>
        /// gets User phone no.
        /// </summary>
        /// <returns> change phone partial view </returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<PartialViewResult> EditPhoneNumber()
        {
            UpdatePhone phone = await _user.getPhoneNumber(User.Identity.Name);
            return PartialView("_ChangePhone", phone);
        }
    }
}