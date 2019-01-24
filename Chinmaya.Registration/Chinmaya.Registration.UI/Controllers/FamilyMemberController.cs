using Chinmaya.Registration.Models;
using Chinmaya.Registration.UI.Services;
using Chinmaya.Registration.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Chinmaya.Registration.UI.Controllers
{
    [CustomAuthorize(Roles = "User")]
    public class FamilyMemberController : BaseController
    {
        CommonService _common = new CommonService();
        AccountService _account = new AccountService();
        UserService _user = new UserService();
        /// <summary>
        /// adds family member
        /// </summary>
        /// <param name="MemberInformation"> Family Member Model </param>
        /// <returns> My Account view </returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> AddFamilyMember(FamilyMemberModel MemberInformation)
        {
            ToastModel tm = new ToastModel();
            MemberInformation.relationships = await _common.GetRelationshipData();
            MemberInformation.grades = await _common.GetGradeData();
            MemberInformation.genders = await _common.GetGenderData();
            if (ModelState.IsValid)
            {
                MemberInformation.UpdatedBy = User.UserId;
                bool isEmailExists = string.IsNullOrEmpty(MemberInformation.Email) ? false : await _account.CheckIsEmailExists(MemberInformation.Email);
                if (!isEmailExists)
                {
                   HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/User/PostFamilyMember", MemberInformation, true);
                   tm.IsSuccess = true;
                   tm.Message = "Family member added/updated successfully";
                }
                else
                {
                   tm.IsSuccess = false;
                   tm.Message = "User Already Exists";
                }
                return Json(tm);
            }
            return RedirectToAction("MyAccount", "Account");
        }

        /// <summary>
        /// gets family member partial view with data binding
        /// </summary>
        /// <returns> family member partial view </returns>
		[HttpGet]
        [AllowAnonymous]
        public async Task<PartialViewResult> RefreshFamilyMemberPartialView()
        {
            FamilyMemberModel fm = new FamilyMemberModel();
			DateTime todaysDate = DateTime.Now.Date;
			int day = todaysDate.Day;
			int month = todaysDate.Month;
			int year = todaysDate.Year;
			if (month >= 6)
				fm.Year = year;
			else if (month < 6)
				fm.Year = year - 1;
			fm.relationships = await _common.GetRelationshipData();
            fm.grades = await _common.GetGradeData();
            fm.genders = await _common.GetGenderData();
            return PartialView("_AddFamilyMember", fm);
        }

        /// <summary>
        /// gets family member details
        /// </summary>
        /// <param name="Id"> family member id </param>
        /// <returns> family member partial view </returns>
        public async Task<PartialViewResult> EditFamilyMember(string Id)
        {
            FamilyMemberModel fm = await _user.FamilyMemberDetails(Id);
			DateTime todaysDate = DateTime.Now.Date;
			int day = todaysDate.Day;
			int month = todaysDate.Month;
			int year = todaysDate.Year;
			if (month >= 6) fm.Year = year;
			else if (month < 6) fm.Year = year - 1;
			fm.Id = Id;
            fm.relationships = await _common.GetRelationshipData();
            fm.grades = await _common.GetGradeData();
            fm.genders = await _common.GetGenderData();

            return PartialView("_AddFamilyMember", fm);
        }
	}
}