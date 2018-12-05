﻿using Chinmaya.Registration.Models;
using Chinmaya.Registration.UI.Providers;
using Chinmaya.Registration.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using Chinmaya.Utilities;
using System.Configuration;
using Chinmaya.Registration.UI.Services;

namespace Chinmaya.Registration.UI.Controllers
{
    [Authorize]
	public class AccountController : BaseController
	{
        AccountService _account = new AccountService();
        UserService _user = new UserService();
        CommonService _common = new CommonService();
        EventService _event = new EventService();

        System.Collections.Specialized.NameValueCollection configMngr = ConfigurationManager.AppSettings;
        /// <summary>
        /// Loads login interface
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns> Login View </returns>
        [AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
            ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		/// <summary>
        /// 
        /// </summary>
        /// <param name="model">Login View Model</param>
        /// <param name="returnUrl"></param>
        /// <returns> returns view according to the user role </returns>
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl = "")
		{
			if (ModelState.IsValid)
			{
                EncryptDecrypt objEncryptDecrypt = new EncryptDecrypt();
				model.Password = objEncryptDecrypt.Encrypt(model.Password, WebConfigurationManager.AppSettings["ServiceAccountPassword"]);
				Utility.MasterType masterValue = Utility.MasterType.ROLE;
				HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/Master/GetMasterData", masterValue, true);
				HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/User/", model, true);

				if (userResponseMessage.IsSuccessStatusCode && roleResponseMessage.IsSuccessStatusCode)
				{
					var user = await Utility.DeserializeObject<UserModel>(userResponseMessage);
					if (user != null)
					{
                        if (!user.EmailConfirmed)
                        {
                            ViewBag.IsUserActivated = false;
                            ViewBag.UserNotActivated = "Please verify your registered email address and try to login again.";
                            return View("Login");
                        }
						HttpResponseMessage roleNameResponseMessage = await Utility.GetObject("/api/User/" + user.RoleId, true);
						string roleName = await Utility.DeserializeObject<string>(roleNameResponseMessage);
                        List<string> userRoles = new List<string>{ roleName };

						CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel();
						serializeModel.UserId = user.Id;
						serializeModel.FirstName = user.FirstName;
						serializeModel.LastName = user.LastName;
						serializeModel.roles = userRoles.ToArray();

						string userData = JsonConvert.SerializeObject(serializeModel);
						FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
						1,
						user.Email,
						DateTime.Now,
						DateTime.Now.AddDays(1),
						false, //pass here true, if you want to implement remember me functionality
						userData);

						string encTicket = FormsAuthentication.Encrypt(authTicket);
						HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
						Response.Cookies.Add(faCookie);
						SessionVar.LoginUser = user;

                        if (!string.IsNullOrEmpty(returnUrl)) return Redirect(returnUrl);
                        else
                        {
                            if (roleName.Contains("Admin"))
                            {
                                return RedirectToAction("Index", "Admin");
                            }
                            else if (roleName.Contains("User"))
                            {
                                return RedirectToAction("MyAccount", "Account");
                            }
                            else
                            {
                                return RedirectToAction("Login", "Account");
                            }
                        }
                    }
				}
			}
			return View(model);
		}

        /// <summary>
        /// Shows "Not Authorized" view
        /// </summary>
        /// <returns>"Not Authorized" view</returns>
        [HttpGet]
        public ActionResult NotAuthorized()
        {
            return View();
        }

		/// <summary>
        /// Shows Forgot password interface
        /// </summary>
        /// <returns> Forgot Password View </returns>
		[AllowAnonymous]
		public ActionResult ForgotPassword()
		{
            ForgotPasswordModel fm = new ForgotPasswordModel();
			return View(fm);
		}

        /// <summary>
        /// Send instructions to reset forgot password through email
        /// </summary>
        /// <param name="model"> Forgot Password view model </param>
        /// <returns> Json string </returns>
       [HttpPost]
       [AllowAnonymous]
        public async Task<JsonResult> ForgotPassword(ForgotPasswordModel model)
        {
            try
            {
                ToastModel tm = new ToastModel();
                bool isEmailExists = await _account.CheckIsEmailExists(model.Email);
                if (isEmailExists)
                {
                    EmailTemplateModel etm = await _account.GetEmailTemplate(8);
                    EncryptDecrypt ed = new EncryptDecrypt();
                    string fullName = await _user.GetUserFullName(model.Email);
                    string forgotPasswordResetLink = configMngr["ResetForgotPasswordLink"] + ed.Encrypt(model.Email, configMngr["ServiceAccountPassword"]);
                    string emaiBody = etm.Body.Replace("[Username]", fullName)
                        .Replace("[URL]", forgotPasswordResetLink);
                    etm.Body = emaiBody;
                    EmailManager em = new EmailManager
                    {
                        Body = etm.Body,
                        To = "dinesh.medikonda@cesltd.com", //model.Email,
                        Subject = etm.Subject,
                        From = ConfigurationManager.AppSettings["SMTPUsername"]
                    };

                    em.Send();

                    tm.Message = "Email sent";
                    tm.IsSuccess = true;
                }
                else
                {
                    tm.Message = "Email not found";
                    tm.IsSuccess = false;
                }

                return Json(tm);
            } catch(Exception ex)
            {
                return Json("");
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"> gets encrypted user email</param>
        /// <param name="isRedirected"> when this method is called from some other action method "isRedirected" parameter
        /// becomes true or false.</param>
        /// <returns> Returns view if email is valid or redirected to login page </returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ResetForgotPassword(string user, bool isRedirected = false)
        {
            EncryptDecrypt objEncryptDecrypt = new EncryptDecrypt();
            string email = objEncryptDecrypt.Decrypt(user, configMngr["ServiceAccountPassword"]);
            if(await _account.CheckIsEmailExists(email))
            {
                string primaryEmailAccount = await _account.GetFamilyPrimaryAccountEmail(email);

                List<SecurityQuestionsModel> sqList = await _account.GetSecurityQuestionsByEmail(email);
                ResetForgotPasswordModel rfpm = new ResetForgotPasswordModel
                {
                    Email = email,
                    SecurityQuestionsModel = sqList,
                    IsRedirected = isRedirected
                };

                return View(rfpm);
            }
            return RedirectToAction("Login");
        }

        /// <summary>
        /// It will resets user password
        /// </summary>
        /// <param name="model">Reset Forgot Password Model</param>
        /// <returns>Reset Password Confirmation view </returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ResetForgotPassword(ResetForgotPasswordModel model)
        {
            bool areAllAnswersValid = false;
            ToastModel tm = new ToastModel();
            List<SecurityQuestionsModel> sqList = await _account.GetSecurityQuestionsByEmail(model.Email);
            Dictionary<int, string> userAnsweredQuestions = new Dictionary<int, string>();

            for (int i = 0; i < sqList.Count; i++)
            {
                if ((Request.Form["AnswerTextbox_" + (i + 1)]) != "")
                {
                    userAnsweredQuestions.Add((i + 1), Request.Form["AnswerTextbox_" + (i + 1)]);
                }

            }

            foreach (var item in userAnsweredQuestions)
            {
                sqList.ForEach(sq =>
                {
                    if (sq.Id == item.Key)
                    {
                        areAllAnswersValid = sq.Value == item.Value;
                    }
                });
            }

            if(areAllAnswersValid)
            {
                ResetPasswordModel rpm = new ResetPasswordModel
                {
                    Email = model.Email,
                    Password = model.Password
                };
                string urlAction = "api/Account/ResetUserPassword";
                HttpResponseMessage resetPasswordResponse = await Utility.GetObject(urlAction, rpm);

                tm.IsSuccess = await Utility.DeserializeObject<bool>(resetPasswordResponse);
                tm.Message = "Your password has been reset";
            }
            else
            {
                EncryptDecrypt objEncryptDecrypt = new EncryptDecrypt();
                string email = objEncryptDecrypt.Encrypt(model.Email, configMngr["ServiceAccountPassword"]);
                return RedirectToAction("ResetForgotPassword", new { user = email, isRedirected = true });
            }

            return View("ResetPasswordConfirmation", tm);
        }

		/// <summary>
        /// Signouts the user from application
        /// </summary>
        /// <returns> Login view </returns>
		[HttpGet]
		public ActionResult LogOff()
		{
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Response.Cookies["userInfo"].Value = "";
            Response.Cookies["userInfo"].Expires = DateTime.Now.AddDays(-1);
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
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
        /// Shows Registration view
        /// </summary>
        /// <returns> Registration View </returns>
		[AllowAnonymous]
		public ActionResult Registration()
		{
			return View();
		}

        /// <summary>
        /// Shows Personal details interface
        /// </summary>
        /// <param name="pd"> Personal Details model </param>
        /// <returns>Personal details View </returns>
		[AllowAnonymous]
        [HttpGet]
		public async Task<ActionResult> PersonalDetails(PersonalDetails pd = null)
		{
            ViewBag.Gender = await _common.GetGenderData();
			ViewBag.SelectedGender = null;
			ViewBag.AgeGroup = await _common.GetAgeGroupData();
			ViewBag.SelectedAgeGroup = null;

            return View();
		}

        /// <summary>
        /// takes personal details data and bind it to the view
        /// </summary>
        /// <param name="DetailsData">Personal Details Model</param>
        /// <param name="BtnPrevious"> Back button name </param>
        /// <param name="BtnNext"> Next button name </param>
        /// <returns> Contact Details view or Personal Dateils view </returns>
		[HttpPost]
		[AllowAnonymous]
		public async Task<ActionResult> PersonalDetails(PersonalDetails DetailsData, string BtnPrevious, string BtnNext)
		{
			ViewBag.Gender = await _common.GetGenderData();
			ViewBag.AgeGroup = await _common.GetAgeGroupData();
			ViewBag.CountryList = await _common.GetCountryData();
			//ViewBag.SelectedCountry = await _account.GetCountryId("United States");
			ViewBag.SelectedCountry = 231;

			if (BtnNext != null)
			{
				if (ModelState.IsValid)
				{
					UserModel UserObj = GetUser();

					UserObj.FirstName = DetailsData.FirstName;
					UserObj.LastName = DetailsData.LastName;
					UserObj.DOB = (DateTime)DetailsData.DOB;
					UserObj.GenderId = DetailsData.GenderData;
					UserObj.AgeGroupId = DetailsData.AgeGroupData;
                    return RedirectToAction("ContactDetails");
				}
			}
			return View();
		}

        /// <summary>
        /// Shows Contact details interface
        /// </summary>
        /// <param name="cd"> Contact Details model </param>
        /// <returns>Contact Details view </returns>
        [HttpGet]
        public async Task<ActionResult> ContactDetails(ContactDetails cd = null)
        {
            ViewBag.CountryList = await _common.GetCountryData();
            ViewBag.SelectedCountry = 231;
            return View(cd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"> Contact Detaila model </param>
        /// <param name="prevBtn"> Back button name </param>
        /// <param name="nextBtn"> Next button name </param>
        /// <returns> Contact Details view or Personal Dateils view or Account Deatails view </returns>
        [HttpPost]
		[AllowAnonymous]
		public async Task<ActionResult> ContactDetails(ContactDetails data, string prevBtn, string nextBtn)
		{
			UserModel obj = GetUser();
			ViewBag.CountryList = await _common.GetCountryData();
			ViewBag.SelectedCountry = 231;

			if (prevBtn != null)
			{
				PersonalDetails pd = new PersonalDetails();
				pd.FirstName = obj.FirstName;
				pd.LastName = obj.LastName;
				pd.DOB = obj.DOB;
				pd.GenderData = obj.GenderId;
				pd.AgeGroupData = (int)obj.AgeGroupId;
				ViewBag.Gender = await _common.GetGenderData();
				ViewBag.SelectedGender = obj.GenderId;
				ViewBag.AgeGroup = await _common.GetAgeGroupData();
				ViewBag.SelectedAgeGroup = (int)obj.AgeGroupId;
                return RedirectToAction("PersonalDetails", pd);
			}

			if (nextBtn != null)
			{
				if (ModelState.IsValid)
				{
					obj.Address = data.Address;
					obj.CountryId = data.Country;
					obj.StateId = data.State;
					obj.City = data.City;
					obj.ZipCode = data.ZipCode;
					obj.HomePhone = data.HomePhone;
					obj.CellPhone = data.CellPhone;
					AccountDetails Ad = new AccountDetails();
					Ad.SecurityQuestionsModel = await _common.GetSecurityQuestions();
                    return RedirectToAction("AccountDetails");
				}
			}
			return View();
		}

        /// <summary>
        /// Shows Account Details interface
        /// </summary>
        /// <returns> Account Details view </returns>
        [HttpGet]
        public async Task<ActionResult> AccountDetails()
        {
            AccountDetails Ad = new AccountDetails();
            ViewBag.SecurityQuestions = await _common.GetSecurityQuestions();
            return View(Ad);
        }

        /// <summary>
        /// Takes Account details model data and bind it to the view 
        /// </summary>
        /// <param name="data"> Account details model </param>
        /// <param name="prevBtn"> Back button name </param>
        /// <param name="nextBtn"> Next button name </param>
        /// <returns> Contact Details view or Account details view or Login view </returns>
        [HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> AccountDetails(AccountDetails data, string prevBtn, string nextBtn)
		{
			UserModel obj = GetUser();
            ToastModel tm = new ToastModel();
			SecurityQuestionsModel Sqm = new SecurityQuestionsModel();

            ContactDetails cd = new ContactDetails();
            cd.Address = obj.Address;
            cd.Country = obj.CountryId;
            ViewBag.CountryList = await _common.GetCountryData();
            ViewBag.SelectedCountry = obj.CountryId;
            cd.State = obj.StateId;
            ViewBag.SelectedState = obj.StateId;
            cd.City = obj.City;
            cd.ZipCode = obj.ZipCode;
            cd.HomePhone = obj.HomePhone;
            cd.CellPhone = obj.CellPhone;

            if (prevBtn != null)
			{
                //return View("ContactDetails", cd);
                return RedirectToAction("ContactDetails", cd);
			}
			if (nextBtn != null)
			{
				Dictionary<int, string> SecurityQuestions = new Dictionary<int, string>();

				for (int i = 0; i < 5; i++)
				{
					if ((Request.Form["AnswerTextbox_" + (i + 1)]) != "")
					{
						SecurityQuestions.Add((i + 1), Request.Form["AnswerTextbox_" + (i + 1)]);
					}

				}

				AccountDetails Ad = new AccountDetails();
				Ad.Email = data.Email;
				Ad.Password = data.Password;
				Ad.RetypePassword = data.RetypePassword;
				Ad.AccountType = data.AccountType;
				Ad.SecurityQuestionsModel = await _common.GetSecurityQuestions();

				foreach (var item in SecurityQuestions)
				{
					Ad.SecurityQuestionsModel.ForEach(sq =>
					{
						if (sq.Id == item.Key)
						{
							sq.Value = item.Value;
						}
					});
				}

				if (SecurityQuestions.Count < 2)
				{
					return View("AccountDetails", Ad);
				}

				else
				{
					if (ModelState.IsValid)
					{
                        bool userRejected = false;
                        bool isEmailExists = await _account.CheckIsEmailExists(data.Email);
                        bool isFamilyMember = await _account.IsFamilyMember(data.Email);
                        bool isAddressOrHomePhoneMatched = await _account.IsAddressOrHomePhoneMatched(cd);
                        if (isEmailExists)
                        {
                            tm.IsSuccess = false;
                            tm.Message = "Email already registered";
                            ViewBag.Toast = tm;
                            return View("AccountDetails", Ad);
                        }

                        obj.Email = data.Email;
                        obj.Password = data.Password;
                        EncryptDecrypt objEncryptDecrypt = new EncryptDecrypt();
                        obj.Password = objEncryptDecrypt.Encrypt(data.Password, WebConfigurationManager.AppSettings["ServiceAccountPassword"]);
                        obj.IsIndividual = Convert.ToBoolean(data.AccountType);
                        obj.UserSecurityQuestions = SecurityQuestions;
                        obj.Status = false;

                        // case: If user is added as a member of someone else's family
                        // send approval mail to primary account holder and user should be in inactive status until request has approved.
                        if (isFamilyMember || isAddressOrHomePhoneMatched)
                        {
                            obj.IsIndividual = true;
                            obj.IsApproveMailSent = true;
                            int emailTemplateId = isFamilyMember ? 2 : 9;
                            // if user has already requested for logins and again trying to get register
                            UserModel um = await _user.GetUserInfo(data.Email);
                            if(!string.IsNullOrEmpty(um.Id))
                            {
                                // if primary a/c holder Rejected the user request
                                if ((bool)um.IsApproveMailSent && um.IsApproved != null)
                                {
                                    userRejected = true;
                                    // so we have to reset the approval request
                                    um.IsApproved = null;
                                    // we need to update the user
                                    HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/User/PostUser", um, true);
                                }

                                // approval mail sent to primary a/c and not yet decided
                                if ((bool)um.IsApproveMailSent && um.IsApproved == null && !userRejected)
                                {
                                    ViewBag.ApproveMailSent = true;
                                    ViewBag.ApproveContent = "An approval email has been already sent to primary account holder of your family..! Please be patient until your request has been approved.";
                                    return View("AccountDetails", Ad);
                                }
                                
                            }

                            ViewBag.IsFamilyMember = true;
                            EmailTemplateModel etm1 = await _account.GetEmailTemplate(emailTemplateId);
                            string toUserFullname = await _user.GetUserFullName(data.Email);
                            string primaryAccountEmail = await _account.GetFamilyPrimaryAccountEmail(data.Email);
                            string fromUserFullname = await _user.GetUserFullName(primaryAccountEmail);

                            string approvalLink1 = configMngr["SharedAccountRequestLink"]
                                + objEncryptDecrypt.Encrypt(data.Email, configMngr["ServiceAccountPassword"])
                                + "&aadm="
                                + isAddressOrHomePhoneMatched;

                            string emailBody1 = etm1.Body
                                .Replace("[ToUsername]", toUserFullname)
                                .Replace("[FromUsername]", fromUserFullname)
                                .Replace("[URL]", approvalLink1);
                            etm1.Body = emailBody1;

                            EmailManager em1 = new EmailManager
                            {
                                Body = etm1.Body,
                                To = "dinesh.medikonda@cesltd.com", // make it as dynamic
                                Subject = etm1.Subject,
                                From = ConfigurationManager.AppSettings["SMTPUsername"]
                            };
                            em1.Send();

                            ViewBag.ApproveContent = "An approval email has been sent to primary account holder of your family..! Your account will be activated once your request has been approved.";
                            if (!userRejected)
                            {
                                HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/User/PostUser", obj, true);
                            }

                            return View("AccountDetails", Ad);
                        }

                        // If user is not a family member, allow him to register normally
                        HttpResponseMessage userResponseMessage1 = await Utility.GetObject("/api/User/PostUser", obj, true);

                        // if user registered successfully, then send an activation link
                        if (userResponseMessage1.IsSuccessStatusCode)
                        {
                            EmailTemplateModel etm = await _account.GetEmailTemplate(1);
                            string approvalLink = configMngr["UserActivationLink"]
                                + objEncryptDecrypt.Encrypt(data.Email, configMngr["ServiceAccountPassword"]);
                            string fullname = obj.FirstName + " " + obj.LastName;
                            string emailBody = etm.Body
                                .Replace("[Username]", fullname)
                                .Replace("[URL]", approvalLink);
                            etm.Body = emailBody;

                            EmailManager em = new EmailManager
                            {
                                Body = etm.Body,
                                To = "dinesh.medikonda@cesltd.com", // make it as dynamic
                                Subject = etm.Subject,
                                From = ConfigurationManager.AppSettings["SMTPUsername"]
                            };
                            em.Send();
                        }
						return RedirectToAction("Login", "Account");
					}
				}
			}

			return View();
		}

        /// <summary>
        /// Shows user activation success or failed
        /// </summary>
        /// <param name="user"> encrypted user email </param>
        /// <returns> User activation view </returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> UserActivation(string user)
        {
            string email = _common.DecryptContent(user);
            UserModel um = await _user.GetUserInfo(email);
            ToastModel tm = new ToastModel();
            if(um != null)
            {
                um.EmailConfirmed = true;
                HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/User/PostUser", um, true);
                if (userResponseMessage.IsSuccessStatusCode)
                {
                    tm.IsSuccess = true;
                    tm.Message = "Your account has been activated successfully";
                }
                else
                {
                    tm.IsSuccess = false;
                    tm.Message = "Problem occurred while activating your account, try again..";
                }
            }
            else
            {
                tm.IsSuccess = false;
                tm.Message = "User not found";
            }

            return View(tm);
        }

        /// <summary>
        /// Shows shared account request interface
        /// </summary>
        /// <param name="user"> encrypted user email </param>
        /// <param name="aadm"> Are user address details matched or not </param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> SharedAccountRequest(string user, bool aadm = false)
        {
            EncryptDecrypt objEncryptDecrypt = new EncryptDecrypt();
            string email = objEncryptDecrypt.Decrypt(user, configMngr["ServiceAccountPassword"]);

            ApproveRejectModel arm = new ApproveRejectModel();
            arm.FullName = await _user.GetUserFullName(email);
            arm.Email = email;
            arm.AreAddressDetailsMatched = aadm;

            return View(arm);
        }

        /// <summary>
        /// Sends email to the user with approved/rejected status
        /// </summary>
        /// <param name="arm">Approve Reject Model</param>
        /// <returns> Is success or failed json result </returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> SharedAccountRequest(ApproveRejectModel arm)
        {
            UserModel um = await _user.GetUserInfo(arm.Email);
            um.IsApproved = arm.IsApproved;
            um.Status = arm.IsApproved ? true : false;
            HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/User/PostUser", um, true);

            string status = arm.IsApproved ? "Approved" : "Rejected";
            if (userResponseMessage.IsSuccessStatusCode)
            {
                EmailTemplateModel etm = await _account.GetEmailTemplate(3);
                string primaryAccountEmail = await _account.GetFamilyPrimaryAccountEmail(arm.Email);
                string fromUserFullname = await _user.GetUserFullName(primaryAccountEmail);

                string emailSubject = etm.Subject
                    .Replace("[Status]", status);
                etm.Subject = emailSubject;

                string emailBody = etm.Body
                    .Replace("[FromUsername]", fromUserFullname)
                    .Replace("[ToUsername]", arm.FullName)
                    .Replace("[AcceptanceStatus]", status);
                etm.Body = emailBody;

                EmailManager em = new EmailManager
                {
                    Body = etm.Body,
                    To = "dinesh.medikonda@cesltd.com", // make it as dynamic
                    Subject = etm.Subject,
                    From = ConfigurationManager.AppSettings["SMTPUsername"]
                };
                em.Send();

                if(arm.AreAddressDetailsMatched && arm.IsApproved)
                {
                    FamilyMemberModel fm = new FamilyMemberModel();
                    fm.CellPhone = um.CellPhone;
                    fm.DOB = um.DOB;
                    fm.Email = um.Email;
                    fm.FirstName = um.FirstName;
                    fm.GenderData = um.GenderId;
                    fm.LastName = um.LastName;
                    fm.RelationshipData = 6;
                    fm.UpdatedBy = await _account.GetUserIdByEmail(um.Email);

                    HttpResponseMessage addFamilyMemberRes = await Utility.GetObject("/api/User/PostFamilyMember", fm, true);
                }
            }
            return Json(new { IsSuccess = userResponseMessage.IsSuccessStatusCode });
        }

        /// <summary>
        /// Gets all the states
        /// </summary>
        /// <param name="Id"> Country Id </param>
        /// <returns>serialized States json result </returns>
        [HttpGet]
		[AllowAnonymous]
		public async Task<JsonResult> FillState(int Id)
		{
			Utility.MasterType masterValue = Utility.MasterType.STATE;
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/Master/GetMasterData", masterValue, true);
			HttpResponseMessage roleNameResponseMessage = await Utility.GetObject("/api/Account/GetState/" + Id, true);
			var serializedStates = await Utility.DeserializeList<KeyValueModel>(roleNameResponseMessage);
			return Json(serializedStates, JsonRequestBehavior.AllowGet);
		}

        /// <summary>
        /// Gets all the Cities
        /// </summary>
        /// <param name="Id"> State Id </param>
        /// <returns>serialized Cities json result </returns>
		[HttpGet]
		[AllowAnonymous]
		public async Task<JsonResult> FillCity(int Id)
		{
			Utility.MasterType masterValue = Utility.MasterType.CITY;
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/Master/GetMasterData", masterValue, true);
			HttpResponseMessage roleNameResponseMessage = await Utility.GetObject("/api/Account/GetCity/" + Id, true);
			var serializedCities = await Utility.DeserializeList<KeyValueModel>(roleNameResponseMessage);
			return Json(serializedCities, JsonRequestBehavior.AllowGet);
		}

        /// <summary>
        /// Shows Family members details/self details 
        /// </summary>
        /// <param name="tm"> Toast message model </param>
        /// <returns> My account view </returns>
		[AllowAnonymous]
		public async Task<ActionResult> MyAccount(ToastModel tm = null)
		{
			ViewBag.Fullname = await _user.GetUserFullName(User.Identity.Name);
			if (!string.IsNullOrEmpty(tm.Message))
            {
                ViewBag.tm = tm;
            }
			MyAccountModel myAccountModel = new MyAccountModel();
			myAccountModel.userFamilyMember = await _user.GetUserFamilyMemberData(User.UserId);
			myAccountModel.familyMemberModel.relationships = await _common.GetRelationshipData();
			myAccountModel.familyMemberModel.grades = await _common.GetGradeData();
			myAccountModel.familyMemberModel.genders = await _common.GetGenderData();
			myAccountModel.IsIndividual = await _account.GetIsIndividual(User.UserId);
			ViewBag.CountryList = await _common.GetCountryData();

			DateTime todaysDate = DateTime.Now.Date;
			int day = todaysDate.Day;
			int month = todaysDate.Month;
			int year = todaysDate.Year;
			if (month >= 6)
				myAccountModel.familyMemberModel.Year = year;
			else if(month < 6)
				myAccountModel.familyMemberModel.Year = year-1;
			
			return View("MyAccount", myAccountModel);
		}

        /// <summary>
        /// adds family member
        /// </summary>
        /// <param name="MemberInformation"> Family Member Model </param>
        /// <param name="nextBtn"> submit button name </param>
        /// <returns> My Account view </returns>
		[HttpPost]
		[AllowAnonymous]
		public async Task<ActionResult> AddFamilyMember(FamilyMemberModel MemberInformation, string nextBtn)
		{
            ToastModel tm = new ToastModel();
			MemberInformation.relationships = await _common.GetRelationshipData();
			MemberInformation.grades = await _common.GetGradeData();
			MemberInformation.genders = await _common.GetGenderData();
			if (nextBtn != null)
			{
				if (ModelState.IsValid)
				{
					MemberInformation.UpdatedBy = User.UserId;

                    bool isEmailExists = string.IsNullOrEmpty(MemberInformation.Email) ? false : await _account.CheckIsEmailExists(MemberInformation.Email);
                    if (!isEmailExists)
                    {
						HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/User/PostFamilyMember", MemberInformation, true);
						tm.IsSuccess = true;
                        tm.Message = "Family member added/updated successfully";
                    } else
                    {
                        tm.IsSuccess = false;
                        tm.Message = "User Already Exists";
                    }
                    
                    return Json(tm);
				}
			}
			return RedirectToAction("MyAccount");
		}

        /// <summary>
        /// Changes user account password
        /// </summary>
        /// <param name="Info"> Update Password Model</param>
        /// <param name="nextBtn"> </param>
        /// <returns> My Account view </returns>
		[HttpPost]
		[AllowAnonymous]
		public async Task<ActionResult> ChangePassword(UpdatePasswordModel Info, string nextBtn)
		{
			ToastModel tm = new ToastModel();
			if (nextBtn != null)
			{
				if (ModelState.IsValid)
				{
					if (Info.OldPassword == Info.NewPassword)
					{
						tm.IsSuccess = true;
						tm.Message = "Please give new Password that should not match the Old";
						return Json(tm);
					}
					else
					{
						UpdatePasswordModel passwordModel = new UpdatePasswordModel();
						passwordModel.Email = User.Identity.Name;
						EncryptDecrypt objEncryptDecrypt = new EncryptDecrypt();
						passwordModel.OldPassword = objEncryptDecrypt.Encrypt(Info.OldPassword, WebConfigurationManager.AppSettings["ServiceAccountPassword"]);
						EncryptDecrypt objEncryptDecrypt1 = new EncryptDecrypt();
						passwordModel.NewPassword = objEncryptDecrypt.Encrypt(Info.NewPassword, WebConfigurationManager.AppSettings["ServiceAccountPassword"]);
						HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/User/UpdatePassword", passwordModel, true);
						tm.IsSuccess = true;
						tm.Message = "Password updated successfully";
						return Json(tm);
					}
				}
			}
			return RedirectToAction("MyAccount");
		}

        /// <summary>
        /// Changes user phone no.
        /// </summary>
        /// <param name="Info"> Update phone model </param>
        /// <param name="nextBtn"></param>
        /// <returns> My Account view </returns>
		[HttpPost]
		[AllowAnonymous]
		public async Task<ActionResult> ChangePhone(UpdatePhone Info, string nextBtn)
		{
			ToastModel tm = new ToastModel();
			if (nextBtn != null)
			{
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
			}
			return RedirectToAction("MyAccount");
		}

        /// <summary>
        /// Changes User account email
        /// </summary>
        /// <param name="em">Update Email model </param>
        /// <param name="nextBtn"></param>
        /// <returns> My Account view </returns>
		[HttpPost]
		[AllowAnonymous]
		public async Task<ActionResult> ChangeEmail(UpdateEmail em, string nextBtn)
		{
			ToastModel tm = new ToastModel();
			if (nextBtn != null)
			{
				if (ModelState.IsValid)
				{
					string uId = await _account.GetUserIdByEmail(User.Identity.Name);
					bool isEmailExists = await _account.CheckIsEmailExists(em.email);
					if (isEmailExists)
					{
						tm.IsSuccess = true;
						tm.Message = "Select Email which does not exist already...!";
						return Json(tm);
					}
					else
					{
						UpdateEmail emailModel = new UpdateEmail();
						emailModel.userId = uId;
						emailModel.email = em.email;
						HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/User/UpdateEmailAddress", emailModel, true);
						tm.IsSuccess = true;
						tm.Message = "Email updated successfully";
						return Json(tm);
					}
				}
			}
			return RedirectToAction("MyAccount");
		}

        /// <summary>
        /// Changes user address
        /// </summary>
        /// <param name="Info"> Contact Details model </param>
        /// <param name="nextBtn"></param>
        /// <returns> My Account view </returns>
		[HttpPost]
		[AllowAnonymous]
		public async Task<ActionResult> ChangeAddress(ContactDetails Info, string nextBtn)
		{
			ToastModel tm = new ToastModel();
			if (nextBtn != null)
			{
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
			}
			return RedirectToAction("MyAccount");
		}

        /// <summary>
        /// gets family member partial view with data binding
        /// </summary>
        /// <returns> family member partial view </returns>
		[HttpGet]
		[AllowAnonymous]
		public async Task<PartialViewResult> RefreshFamilyMemberPartialView() {
			FamilyMemberModel fm = new FamilyMemberModel();
			fm.relationships = await _common.GetRelationshipData();
			fm.grades = await _common.GetGradeData();
			fm.genders = await _common.GetGenderData();
			return PartialView("_AddFamilyMember", fm);
		}

        /// <summary>
        /// gets User phone no.
        /// </summary>
        /// <param name="Email"> user email </param>
        /// <returns> change phone partial view </returns>
		[HttpGet]
		[AllowAnonymous]
		public async Task<PartialViewResult> EditPhoneNumber(string Email)
		{
			UpdatePhone phone = await _user.getPhoneNumber(Email);
			return PartialView("_ChangePhone", phone);
		}

        /// <summary>
        /// gets user email
        /// </summary>
        /// <param name="Email"> user email </param>
        /// <returns> Change Email partial view </returns>
		[HttpGet]
		[AllowAnonymous]
		public async Task<PartialViewResult> EditEmail(string Email)
		{
			UpdateEmail email = await _user.getEmail(Email);
			return PartialView("_ChangeEmail", email);
		}

        /// <summary>
        /// gets user address
        /// </summary>
        /// <param name="Email"> user email </param>
        /// <returns> Change Address partial view </returns>
		[HttpGet]
		[AllowAnonymous]
		public async Task<PartialViewResult> EditAddress(string Email)
		{
			ContactDetails cd = await _user.getAddress(Email);
			ViewBag.CountryList = await _common.GetCountryData();
			ViewBag.SelectedCountry = cd.Country;
			ViewBag.SelectedState = cd.State;
			return PartialView("_ChangeAddress", cd);
		}

        /// <summary>
        /// gets family member details
        /// </summary>
        /// <param name="Id"> family member id </param>
        /// <returns> family member partial view </returns>
		public async Task<PartialViewResult> EditFamilyMember(string Id)
		{
            FamilyMemberModel fm = await _user.FamilyMemberDetails(Id);

			fm.Id = Id;
			fm.relationships = await _common.GetRelationshipData();
			fm.grades = await _common.GetGradeData();
			fm.genders = await _common.GetGenderData();

			return PartialView("_AddFamilyMember", fm);
		}
	}
}