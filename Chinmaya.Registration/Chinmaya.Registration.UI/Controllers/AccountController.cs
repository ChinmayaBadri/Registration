using Chinmaya.DAL;
using Chinmaya.Models;
using Chinmaya.Registration.DAL;
using Chinmaya.Registration.Models;
using Chinmaya.Registration.UI.Providers;
using Chinmaya.Registration.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using Chinmaya.Utilities;
using System.Configuration;
using System.Net.Mail;
using System.Net;

namespace Chinmaya.Registration.UI.Controllers
{
	[Authorize]
	public class AccountController : BaseController
	{
		Users _user = new Users();
        System.Collections.Specialized.NameValueCollection configMngr = ConfigurationManager.AppSettings;
        //
        // GET: /Account/Login
        [AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
            ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		//
		// POST: /Account/Login
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
				HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/MasterAPI/GetMasterData", masterValue, true);
				HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/UserAPI/", model, true);

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
						HttpResponseMessage roleNameResponseMessage = await Utility.GetObject("/api/UserAPI/" + user.RoleId, true);
						string roleName = await Utility.DeserializeObject<string>(roleNameResponseMessage);
                        //var serializedRoles = await Utility.DeserializeList<KeyValueModel>(roleResponseMessage);
                        //var roles = serializedRoles.Select(c => c.Name).ToArray<string>();

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
                                return RedirectToAction("Admin", "Account");
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


		[CustomAuthorize(Roles = "admin")]
		public ActionResult Admin()
		{
			//var msg = await ChangeAccountType("6278B716-60D9-4CBD-A7D3-2010CEB36C10");
			return View();
		}

        [HttpGet]
        public ActionResult NotAuthorized()
        {
            return View();
        }

        [CustomAuthorize(Roles = "admin")]
        public async Task<ActionResult> GetAllUsers()
		{
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/UserAPI/GetAllUsers", true);
			var users = await Utility.DeserializeObject<List<UserInfoModel>>(roleResponseMessage);
			return Json(new { data = users }, JsonRequestBehavior.AllowGet);
		}

        [CustomAuthorize(Roles = "admin")]
        public async Task<ActionResult> GetAllFamilyMembers(string Id)
		{
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/UserAPI/GetAllFamilyMembers/" + Id, true);
			var familyMembers = await Utility.DeserializeObject<List<UFamilyMember>>(roleResponseMessage);
			return Json(new { data = familyMembers }, JsonRequestBehavior.AllowGet);
		}

        [CustomAuthorize(Roles = "admin")]
        public async Task<ActionResult> ChangeAccountType(string Id)
		{
			HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/UserAPI/ChangeAccountType/" + Id, Id, true);
			return RedirectToAction("Admin");
		}
		
		//
		// GET: /Account/Register
		[AllowAnonymous]
		public ActionResult Register()
		{
			return View();
		}

		//
		// POST: /Account/Register
		//[HttpPost]
		//[AllowAnonymous]
		//[ValidateAntiForgeryToken]
		//public ActionResult Register()
		//{

		//    // If we got this far, something failed, redisplay form
		//    return View();
		//}

		//
		// GET: /Account/ConfirmEmail
		[AllowAnonymous]
		public ActionResult ConfirmEmail(string userId, string code)
		{
			if (userId == null || code == null)
			{
				return View("Error");
			}
			return View("Error");
		}

		//
		// GET: /Account/ForgotPassword
		[AllowAnonymous]
		public ActionResult ForgotPassword()
		{
            ForgotPasswordModel fm = new ForgotPasswordModel();
			return View(fm);
		}

       [HttpPost]
       [AllowAnonymous]
        public async Task<JsonResult> ForgotPassword(ForgotPasswordModel model)
        {
            try
            {
                ToastModel tm = new ToastModel();
                bool isEmailExists = await CheckIsEmailExists(model.Email);
                if (isEmailExists)
                {
                    EmailTemplateModel etm = await GetEmailTemplate(8);
                    EncryptDecrypt ed = new EncryptDecrypt();
                    string fullName = await GetUserFullName(model.Email);
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

        public async Task<string> GetUserFullName(string email)
        {
            string urlAction = "api/Account/GetUserFullNameByEmail/" + email + "/";
            HttpResponseMessage getFullnameResponse = await Utility.GetObject(urlAction);

            return await Utility.DeserializeObject<string>(getFullnameResponse);
        }

        public async Task<EmailTemplateModel> GetEmailTemplate(int id)
        {
            string urlAction = "api/Account/GetEmailTemplateByID/" + id;

            HttpResponseMessage emailTemplateResponse = await Utility.GetObject(urlAction);
            return await Utility.DeserializeObject<EmailTemplateModel>(emailTemplateResponse);
        }

        public async Task<string> GetFamilyPrimaryAccountEmail(string email)
        {
            string urlAction = "api/Account/GetFamilyPrimaryAccountEmail/" + email + "/";
            HttpResponseMessage getPrimaryEmailResponse = await Utility.GetObject(urlAction);

            return await Utility.DeserializeObject<string>(getPrimaryEmailResponse);
        }

        public async Task<UserModel> GetUserInfo(string email)
        {
            string urlAction = "api/Account/GetUserInfoByEmail/" + email + "/";
            HttpResponseMessage getUserInfoResponse = await Utility.GetObject(urlAction);

            return await Utility.DeserializeObject<UserModel>(getUserInfoResponse);
        }

        public async Task<bool> IsAddressOrHomePhoneMatched(ContactDetails cd)
        {
            string urlAction = "api/Account/AreAddressDetailsMatched";
            HttpResponseMessage areDetailsMatchedRes = await Utility.GetObject(urlAction, cd, true);

            return await Utility.DeserializeObject<bool>(areDetailsMatchedRes);
        }

        public async Task<string> GetUserIdByEmail(string email)
        {
            string urlAction = "api/Account/GetUserIdByEmail/" + email + "/";
            HttpResponseMessage getUserIDResponse = await Utility.GetObject(urlAction);

            return await Utility.DeserializeObject<string>(getUserIDResponse);
        }
        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
		public ActionResult ForgotPasswordConfirmation()
		{
			return View();
		}

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ResetForgotPassword(string user, bool isRedirected = false)
        {
            EncryptDecrypt objEncryptDecrypt = new EncryptDecrypt();
            string email = objEncryptDecrypt.Decrypt(user, configMngr["ServiceAccountPassword"]);
            if(await CheckIsEmailExists(email))
            {
                string primaryEmailAccount = await GetFamilyPrimaryAccountEmail(email);

                List<SecurityQuestionsModel> sqList = await GetSecurityQuestionsByEmail(email);
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

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ResetForgotPassword(ResetForgotPasswordModel model)
        {
            bool areAllAnswersValid = false;
            ToastModel tm = new ToastModel();
            List<SecurityQuestionsModel> sqList = await GetSecurityQuestionsByEmail(model.Email);
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

        public async Task<List<SecurityQuestionsModel>> GetSecurityQuestionsByEmail(string email)
        {
            string urlAction2 = "api/Account/GetSecurityQuestionsByEmail/" + email + "/";
            HttpResponseMessage getSecurityQuestionsRes = await Utility.GetObject(urlAction2);

            return await Utility.DeserializeList<SecurityQuestionsModel>(getSecurityQuestionsRes);
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
		public ActionResult ResetPassword(string code)
		{
			return code == null ? View("Error") : View();
		}

		//
		// POST: /Account/ResetPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult ResetPassword()
		{

			return View();
		}

		//
		// GET: /Account/ResetPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ResetPasswordConfirmation()
		{
			return View();
		}



		//
		// POST: /Account/LogOff
		[HttpGet]
		public ActionResult LogOff()
		{
            Response.Cookies["userInfo"].Value = "";
            Response.Cookies["userInfo"].Expires = DateTime.Now.AddDays(-1);
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
		}

		[AllowAnonymous]
		private UserModel GetUser()
		{
			if (Session["user"] == null) Session["user"] = new UserModel();
			return (UserModel)Session["user"];
		}

		[AllowAnonymous]
		public ActionResult Registration()
		{
			return View();
		}

		public async Task<List<KeyValueModel>> GetGenderData()
		{
			Utility.MasterType masterValue = Utility.MasterType.GENDER;
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/MasterAPI/GetMasterData", masterValue, true);
			return await Utility.DeserializeObject<List<KeyValueModel>>(roleResponseMessage);
		}

		public async Task<object> GetAgeGroupData()
		{
			Utility.MasterType masterValue = Utility.MasterType.AGEGROUPID;
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/MasterAPI/GetMasterData", masterValue, true);
			return await Utility.DeserializeList<KeyValueModel>(roleResponseMessage);
		}

		[AllowAnonymous]
		public async Task<ActionResult> BindingDataPersonalDetails()
		{
			ViewBag.Gender = await GetGenderData();
			ViewBag.SelectedGender = null;
			ViewBag.AgeGroup = await GetAgeGroupData();
			ViewBag.SelectedAgeGroup = null;
			return View("PersonalDetails");
		}

		public async Task<object> GetCountryData()
		{
			Utility.MasterType masterValue = Utility.MasterType.COUNTRY;
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/MasterAPI/GetMasterData", masterValue, true);
			return await Utility.DeserializeList<KeyValueModel>(roleResponseMessage);
		}

		public async Task<List<SecurityQuestionsModel>> GetSecurityQuestions()
		{
			Utility.MasterType masterValue = Utility.MasterType.SECURITYQUESTIONS;
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/MasterAPI/GetMasterData", masterValue, true);
			return await Utility.DeserializeList<SecurityQuestionsModel>(roleResponseMessage);
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<ActionResult> PersonalDetails(PersonalDetails DetailsData, string BtnPrevious, string BtnNext)
		{
			ViewBag.Gender = await GetGenderData();
			ViewBag.AgeGroup = await GetAgeGroupData();
			ViewBag.CountryList = await GetCountryData();
			ViewBag.SelectedCountry = 231;
			//ViewBag.SelectedState = null;
			if (BtnNext != null)
			{
				if (ModelState.IsValid)
				{
					UserModel UserObj = GetUser();

					UserObj.FirstName = DetailsData.FirstName;
					UserObj.LastName = DetailsData.LastName;
					UserObj.DOB = DetailsData.DOB;
					UserObj.GenderId = DetailsData.GenderData;
					UserObj.AgeGroupId = DetailsData.AgeGroupData;
					return View("ContactDetails");
				}
			}
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<ActionResult> ContactDetails(ContactDetails data, string prevBtn, string nextBtn)
		{
			UserModel obj = GetUser();
			ViewBag.CountryList = await GetCountryData();
			ViewBag.SelectedCountry = 231;
			//ViewBag.SecurityQuestions = await GetSecurityQuestions();
			//List<SecurityQuestionsModel> model = await GetSecurityQuestions();
			//List<SecurityQuestionsModel> model = await GetSecurityQuestions();

			if (prevBtn != null)
			{
				PersonalDetails pd = new PersonalDetails();
				pd.FirstName = obj.FirstName;
				pd.LastName = obj.LastName;
				pd.DOB = obj.DOB;
				pd.GenderData = obj.GenderId;
				pd.AgeGroupData = (int)obj.AgeGroupId;
				ViewBag.Gender = await GetGenderData();
				ViewBag.SelectedGender = obj.GenderId;
				ViewBag.AgeGroup = await GetAgeGroupData();
				ViewBag.SelectedAgeGroup = (int)obj.AgeGroupId;
				return View("PersonalDetails", pd);
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
					//SecurityQuestionsModel Sqm = new SecurityQuestionsModel();
					Ad.SecurityQuestionsModel = await GetSecurityQuestions();
					return View("AccountDetails", Ad);
				}
			}
			return View();
		}

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
            ViewBag.CountryList = await GetCountryData();
            ViewBag.SelectedCountry = obj.CountryId;
            cd.State = obj.StateId;
            ViewBag.SelectedState = obj.StateId;
            cd.City = obj.City;
            cd.ZipCode = obj.ZipCode;
            cd.HomePhone = obj.HomePhone;
            cd.CellPhone = obj.CellPhone;

            if (prevBtn != null)
			{
				return View("ContactDetails", cd);
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
				Ad.SecurityQuestionsModel = await GetSecurityQuestions();

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
                        bool isEmailExists = await CheckIsEmailExists(data.Email);
                        bool isFamilyMember = await IsFamilyMember(data.Email);
                        bool isAddressOrHomePhoneMatched = await IsAddressOrHomePhoneMatched(cd);
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
                            UserModel um = await GetUserInfo(data.Email);
                            if(!string.IsNullOrEmpty(um.Id))
                            {
                                // if primary a/c holder Rejected the user request
                                if ((bool)um.IsApproveMailSent && um.IsApproved != null)
                                {
                                    userRejected = true;
                                    // so we have to reset the approval request
                                    um.IsApproved = null;
                                    // we need to update the user
                                    HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/UserAPI/PostUser", um, true);
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
                            EmailTemplateModel etm1 = await GetEmailTemplate(emailTemplateId);
                            string toUserFullname = await GetUserFullName(data.Email);
                            string primaryAccountEmail = await GetFamilyPrimaryAccountEmail(data.Email);
                            string fromUserFullname = await GetUserFullName(primaryAccountEmail);

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
                                HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/UserAPI/PostUser", obj, true);
                            }

                            return View("AccountDetails", Ad);
                        }

                        // If user is not a family member, allow him to register normally
                        HttpResponseMessage userResponseMessage1 = await Utility.GetObject("/api/UserAPI/PostUser", obj, true);

                        // if user registered successfully, then send an activation link
                        if (userResponseMessage1.IsSuccessStatusCode)
                        {
                            EmailTemplateModel etm = await GetEmailTemplate(1);
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

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> UserActivation(string user)
        {
            string email = DecryptContent(user);
            UserModel um = await GetUserInfo(email);
            ToastModel tm = new ToastModel();
            if(um != null)
            {
                um.EmailConfirmed = true;
                HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/UserAPI/PostUser", um, true);
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

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> SharedAccountRequest(string user, bool aadm = false)
        {
            EncryptDecrypt objEncryptDecrypt = new EncryptDecrypt();
            string email = objEncryptDecrypt.Decrypt(user, configMngr["ServiceAccountPassword"]);

            ApproveRejectModel arm = new ApproveRejectModel();
            arm.FullName = await GetUserFullName(email);
            arm.Email = email;
            arm.AreAddressDetailsMatched = aadm;

            return View(arm);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> SharedAccountRequest(ApproveRejectModel arm)
        {
            UserModel um = await GetUserInfo(arm.Email);
            um.IsApproved = arm.IsApproved;
            um.Status = arm.IsApproved ? true : false;
            HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/UserAPI/PostUser", um, true);

            string status = arm.IsApproved ? "Approved" : "Rejected";
            if (userResponseMessage.IsSuccessStatusCode)
            {
                EmailTemplateModel etm = await GetEmailTemplate(3);
                string primaryAccountEmail = await GetFamilyPrimaryAccountEmail(arm.Email);
                string fromUserFullname = await GetUserFullName(primaryAccountEmail);

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
                    fm.UpdatedBy = await GetUserIdByEmail(um.Email);

                    HttpResponseMessage addFamilyMemberRes = await Utility.GetObject("/api/UserAPI/PostFamilyMember", fm, true);
                }
            }
            return Json(new { IsSuccess = userResponseMessage.IsSuccessStatusCode });
        }

        public string DecryptContent(string content)
        {
            EncryptDecrypt objEncryptDecrypt = new EncryptDecrypt();
            return objEncryptDecrypt.Decrypt(content, configMngr["ServiceAccountPassword"]);
        }

        public string EncryptContent(string content)
        {
            EncryptDecrypt objEncryptDecrypt = new EncryptDecrypt();
            return objEncryptDecrypt.Encrypt(content, configMngr["ServiceAccountPassword"]);
        }

        [HttpGet]
		[AllowAnonymous]
		public async Task<JsonResult> FillState(int Id)
		{
			Utility.MasterType masterValue = Utility.MasterType.STATE;
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/MasterAPI/GetMasterData", masterValue, true);
			HttpResponseMessage roleNameResponseMessage = await Utility.GetObject("/api/Account/GetState/" + Id, true);
			var serializedStates = await Utility.DeserializeList<KeyValueModel>(roleNameResponseMessage);
			return Json(serializedStates, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<JsonResult> FillCity(int Id)
		{
			Utility.MasterType masterValue = Utility.MasterType.CITY;
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/MasterAPI/GetMasterData", masterValue, true);
			HttpResponseMessage roleNameResponseMessage = await Utility.GetObject("/api/Account/GetCity/" + Id, true);
			var serializedCities = await Utility.DeserializeList<KeyValueModel>(roleNameResponseMessage);
			return Json(serializedCities, JsonRequestBehavior.AllowGet);
		}

		public async Task<List<KeyValueModel>> GetRelationshipData()
		{
			Utility.MasterType masterValue = Utility.MasterType.RELATIONSHIP;
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/MasterAPI/GetMasterData", masterValue, true);
			return await Utility.DeserializeObject<List<KeyValueModel>>(roleResponseMessage);
		}

		public async Task<List<KeyValueModel>> GetGradeData()
		{
			Utility.MasterType masterValue = Utility.MasterType.GRADE;
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/MasterAPI/GetMasterData", masterValue, true);
			return await Utility.DeserializeObject<List<KeyValueModel>>(roleResponseMessage);
		}

		public async Task<List<UserFamilyMember>> GetUserFamilyMemberData(string Id)
		{
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/UserAPI/GetUserFamilyMemberData/" + Id, true);
			return await Utility.DeserializeObject<List<UserFamilyMember>>(roleResponseMessage);
		}

		public async Task<bool> GetIsIndividual(string Id)
		{
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/UserAPI/GetIsIndividual/" + Id, true);
			return await Utility.DeserializeObject<bool>(roleResponseMessage);
		}

		public async Task<ActionResult> MyAccount(ToastModel tm = null)
		{
			//ViewBag.Relationship = await GetRelationshipData();
			//ViewBag.Grade = await GetGradeData();
			//ViewBag.Gender = await GetGenderData();
            if(!string.IsNullOrEmpty(tm.Message))
            {
                ViewBag.tm = tm;
            }
			MyAccountModel myAccountModel = new MyAccountModel();
			myAccountModel.userFamilyMember = await GetUserFamilyMemberData(User.UserId);
			myAccountModel.familyMemberModel.relationships = await GetRelationshipData();
			myAccountModel.familyMemberModel.grades = await GetGradeData();
			myAccountModel.familyMemberModel.genders = await GetGenderData();
			myAccountModel.IsIndividual = await GetIsIndividual(User.UserId);

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

		//[HttpPost]
		//[AllowAnonymous]
		//public async Task<ActionResult> EditFamilyMember(string id)
		//{
		//	var MemberInformation = await GetFamilyMemberDetails(id);
		//	return RedirectToAction("AddFamilyMember", MemberInformation);
		//}

		//[AllowAnonymous]
		//public ActionResult ChangePasswordView()
		//{
		//	return PartialView("ChangePasswordView");
		//}

		[HttpPost]
		[AllowAnonymous]
		public async Task<ActionResult> AddFamilyMember(FamilyMemberModel MemberInformation, string nextBtn)
		{
            ToastModel tm = new ToastModel();
			MemberInformation.relationships = await GetRelationshipData();
			MemberInformation.grades = await GetGradeData();
			MemberInformation.genders = await GetGenderData();
			if (nextBtn != null)
			{
				if (ModelState.IsValid)
				{
					MemberInformation.UpdatedBy = User.UserId;

                    bool isEmailExists = string.IsNullOrEmpty(MemberInformation.Email) ? false : await CheckIsEmailExists(MemberInformation.Email);
                    if (!isEmailExists)
                    {
						HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/UserAPI/PostFamilyMember", MemberInformation, true);
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

		[HttpGet]
		[AllowAnonymous]
		public async Task<FamilyMember> FamilyMemberDetails(string Id)
		{
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/UserAPI/GetFamilyMemberDetails/" + Id, true);
			return await Utility.DeserializeObject<FamilyMember>(roleResponseMessage);
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<PartialViewResult> RefreshFamilyMemberPartialView() {
			FamilyMemberModel fm = new FamilyMemberModel();
			fm.relationships = await GetRelationshipData();
			fm.grades = await GetGradeData();
			fm.genders = await GetGenderData();
			return PartialView("_AddFamilyMember", fm);
		}

		public async Task<PartialViewResult> EditFamilyMember(string Id)
		{
			var data = await FamilyMemberDetails(Id);
			FamilyMemberModel fm = new FamilyMemberModel();
			fm.Id = Id;
			fm.relationships = await GetRelationshipData();
			fm.grades = await GetGradeData();
			fm.genders = await GetGenderData();
			fm.FirstName = data.FirstName;
			fm.LastName = data.LastName;
			fm.DOB = data.DOB;
			fm.RelationshipData = data.RelationshipId;
			fm.Grade = (int)data.GradeId;
			fm.GenderData = data.GenderId;
			fm.CellPhone = data.CellPhone;
			fm.Email = data.Email;
			return PartialView("_AddFamilyMember", fm);
		}

		//[HttpPost]
		//[AllowAnonymous]
		//public async Task<ActionResult> EditFamilyMemberDetails(FamilyMemberModel MemberInformation, string nextBtn, string Id)
		//{
		//	var familyMemberData = await FamilyMemberDetails(Id);
		//	ToastModel tm = new ToastModel();
		//	MemberInformation.relationships = await GetRelationshipData();
		//	MemberInformation.grades = await GetGradeData();
		//	MemberInformation.genders = await GetGenderData();
		//	if (nextBtn != null)
		//	{
		//		if (ModelState.IsValid)
		//		{
		//			MemberInformation.UpdatedBy = User.UserId;
		//			bool isEmailExists = await CheckIsEmailExists(MemberInformation.Email);
					
		//				HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/UserAPI/EditFamilyMember" + Id, MemberInformation, true);
		//				tm.IsSuccess = true;
		//				tm.Message = "Family member updated successfully";
		//			return Json(tm);
		//		}
		//		return RedirectToAction("MyAccount");
		//	}
		//	return RedirectToAction("MyAccount");
			
		//}

		public async Task<bool> CheckIsEmailExists(string email)
        {
            string urlAction = "api/Account/IsEmailExists/" + email + "/";
            HttpResponseMessage isEmailExistResponse = await Utility.GetObject(urlAction);
            return await Utility.DeserializeObject<bool>(isEmailExistResponse);
        }

        public async Task<bool> IsFamilyMember(string email)
        {
            string urlAction = "api/Account/IsFamilyMember/" + email + "/";
            HttpResponseMessage isEmailExistResponse = await Utility.GetObject(urlAction);
            return await Utility.DeserializeObject<bool>(isEmailExistResponse);
        }

        public async Task<List<Weekdays>> GetWeekdayData()
		{
			Utility.MasterType masterValue = Utility.MasterType.WEEKDAY;
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/MasterAPI/GetMasterData", masterValue, true);
			return await Utility.DeserializeObject<List<Weekdays>>(roleResponseMessage);
		}

		public async Task<List<Frequencies>> GetFrequencyData()
		{
			Utility.MasterType masterValue = Utility.MasterType.FREQUENCY;
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/MasterAPI/GetMasterData", masterValue, true);
			return await Utility.DeserializeObject<List<Frequencies>>(roleResponseMessage);
		}

		public async Task<List<Sessions>> GetSessionData()
		{
			Utility.MasterType masterValue = Utility.MasterType.SESSION;
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/MasterAPI/GetMasterData", masterValue, true);
			return await Utility.DeserializeObject<List<Sessions>>(roleResponseMessage);
		}

		public async Task<List<CurrentEventModel>> GetEvents()
		{
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/UserAPI/GetEventsData/", true);
			return await Utility.DeserializeObject<List<CurrentEventModel>>(roleResponseMessage);
		}

		[AllowAnonymous]
		public async Task<ActionResult> Event()
		{
			//ViewBag.Weekday = await GetWeekdayData();
			//ViewBag.Frequency = await GetFrequencyData();
			//ViewBag.Session = await GetSessionData();
			//ViewBag.Events = await GetEvents();
			MainEventModel mainEventModel = new MainEventModel();
			mainEventModel.currentEventModel = await GetEvents();
			mainEventModel.weekday = await GetWeekdayData();
			mainEventModel.frequencies = await GetFrequencyData();
			mainEventModel.sessions = await GetSessionData();

			return View("Event", mainEventModel);
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<ActionResult> AddEvent(EventsModel data)
		{
			if (ModelState.IsValid)
			{
				data.CreatedBy = User.UserId;
				HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/UserAPI/PostEvent", data, true);
				return RedirectToAction("Event");
			}
			return RedirectToAction("Event");
		}

		public async Task<UserFamilyMember> GetUserData(string Id)
		{
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/UserAPI/GetUserData/" + Id, true);
			return await Utility.DeserializeObject<UserFamilyMember>(roleResponseMessage);
		}

		public async Task<CurrentEventModel> GetEventData(string Id)
		{
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/UserAPI/GetEventData/" + Id, true);
			return await Utility.DeserializeObject<CurrentEventModel>(roleResponseMessage);
		}

		[AllowAnonymous]
		public async Task<ActionResult> ProgramEventRegistration(string[] select, string prevBtn, string nextBtn)
		{
			//UserModel obj = GetUser();
			//ViewBag.obj = obj;
			//ViewBag.EventList = await GetEventsList(User.UserId);
			//ViewBag.Events = await GetEvents();

			ProgramEventRegistrationModel programEventRegistrationModel = new ProgramEventRegistrationModel();
			programEventRegistrationModel.uFamilyMembers = await GetUserFamilyMemberData(User.UserId);
			programEventRegistrationModel.Events = await GetEvents();

			if (prevBtn != null)
			{
				return RedirectToAction("MyAccount");
			}

			else
			{
				if (nextBtn != null)
				{
					List<ClassesConfirmModel> classesConfirm = new List<ClassesConfirmModel>();

					if (select.Length != 0)
					{
						List<string> selectedId = new List<string>();
						List<List<string>> selectedEventId = new List<List<string>>();
						//List<string> selectedEventId = new List<string>();
						for (int i = 0; i < select.Length; i++)
						{
							var arr = select[i].Split('_');
							var ar1 = arr[0];
							var ar2 = arr[1];
							selectedId.Add(ar2);
							//selectedEventId.Add(ar1);

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
							var userData = await GetUserData(entry.Key);
							//var userData = _user.GetUserData(entry.Key);
							currentEvents.Clear();
							foreach (var ev in entry.Value)
							{
								var eventData = await GetEventData(ev);
								currentEvents.Add(eventData);
							}

							//SelectedListModel selectedListModel = new SelectedListModel();
							ClassesConfirmModel classConfirm = new ClassesConfirmModel();
							classConfirm.uFamilyMembers = userData;
							classConfirm.Events = currentEvents;
							classesConfirm.Add(classConfirm);
						}
					}
					
					return View("ClassesConfirm", classesConfirm);
				}
			}
			return View("ProgramEventRegistration", programEventRegistrationModel);
		}

		[AllowAnonymous]
		public ActionResult ClassesConfirm(string prevBtn, string nextBtn)
		{
			//UserModel obj = GetUser();
			//ViewBag.obj = obj;
			//ViewBag.EventList = await GetEventsList(User.UserId);
			//ViewBag.Events = await GetEvents();
			//ViewBag.AccountType = await GetAccountType();
			if (prevBtn != null)
			{
				return RedirectToAction("ProgramEventRegistration");
			}

			else
			{
				if (nextBtn != null)
				{
					var termCheckBox = Request.Form["termsandConditions"];
					return RedirectToAction("PaymentMethod");
				}
			}
			return View();
		}

		[AllowAnonymous]
		public async Task<string> AddtoDirectory(string Id)
		{
			var id = User.UserId;
			//_user.AddtoDirectory(data);
			HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/UserAPI/AddtoDirectory/" + id, Id, true);
			return "ok";
		}

		//[AllowAnonymous]
		//public async Task<ActionResult> ChangeAccountType(string Id)
		//{
		//	HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/UserAPI/ChangeAccountType/" + Id, Id, true);
		//	return RedirectToAction("Admin");
		//}
		[AllowAnonymous]
		public async Task<object> GetAccountType()
		{
			Utility.MasterType masterValue = Utility.MasterType.ACCOUNTTYPE;
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/MasterAPI/GetMasterData", masterValue, true);
			return await Utility.DeserializeList<KeyValueModel>(roleResponseMessage);
		}

		[AllowAnonymous]
		public async Task<ActionResult> PaymentMethod(CheckPaymentModel data, string prevBtn, string nextBtn)
		{
			ViewBag.AccountType = await GetAccountType();
			if (prevBtn != null)
			{
				return RedirectToAction("ProgramEventRegistration");
			}

			else
			{
				if (nextBtn != null)
				{
					if (ModelState.IsValid && data.paymentType == "Check")
					{
						data.CreatedBy = User.UserId;
						HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/UserAPI/PostCheckPayment", data, true);
						return View();
					}
					return View();
				}
			}
			return View();
		}
    }
}