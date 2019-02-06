using Chinmaya.Registration.Models;
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
		/// Loads NotFound Exception
		/// </summary>
		/// <returns>NotFound View</returns>
		[AllowAnonymous]
		public ActionResult NotFound()
		{
			return View();
		}

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
				HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/User/", model, true);
				UserModel adminData = await _user.GetAdminInfo();

				if (userResponseMessage.IsSuccessStatusCode)
				{
                    var user = await Utility.DeserializeObject<UserModel>(userResponseMessage);
					if (user != null)
					{
						if (user.IsLocked != true)
						{
							if (!user.EmailConfirmed)
							{
								ViewBag.IsUserActivated = false;
								ViewBag.UserNotActivated = "Please verify your registered email address and try to login again.";
								return View("Login");
							}

							HttpResponseMessage roleNameResponseMessage = await Utility.GetObject("/api/User/" + user.RoleId, true);
							string roleName = await Utility.DeserializeObject<string>(roleNameResponseMessage);
                            user.RoleName = roleName;
							List<string> userRoles = new List<string> { roleName };

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
						else
						{
							ViewBag.Message = "Your account has been blocked. Please contact administrator(" + adminData.Email + ") to unlock your account.";
							return View(model);
						}
					}

					else
					{
						UserModel userData = await _user.GetUserInfo(model.UserName);
						
						if (userData.IsLocked == true)
						{
							ViewBag.Message = "Your account has been blocked. Please contact administrator(" + adminData.Email + ") to Unlock your Account.";
							return View("Login");
						}
						else
						{
							if (userData.NumberOfAttempts < 3 || userData.NumberOfAttempts == null && userData.IsLocked != true)
							{
								if(userData.NumberOfAttempts == null) userData.NumberOfAttempts = 1;
								else userData.NumberOfAttempts = userData.NumberOfAttempts + 1;
								HttpResponseMessage userResponseMessage1 = await Utility.GetObject("/api/User/PostUser", userData, true);
								ViewBag.Message = "Please verify your details and try to login again.";
								return View(model);
							}
							else
							{
								userData.NumberOfAttempts = 0;
								userData.IsLocked = true;
								HttpResponseMessage userResponseMessage1 = await Utility.GetObject("/api/User/PostUser", userData, true);
								EmailTemplateModel etm = await _account.GetEmailTemplate(10);
								string emailBody = etm.Body
													.Replace("[Admin]", adminData.FirstName + " " + adminData.LastName)
													.Replace("[Username]", userData.FirstName + " " + userData.LastName);
								etm.Body = emailBody;
								EmailManager em = new EmailManager
								{
									Body = etm.Body,
									To = adminData.Email,
									Subject = etm.Subject,
									From = ConfigurationManager.AppSettings["SMTPUsername"]
								};
								em.Send();
								ViewBag.Message = "Your account has been blocked. Please contact administrator(" +adminData.Email + ") to unlock your account.";
								return View(model);
							}
						}
					}
				}
                else
                {
					ViewBag.ErrorMsg = "Unable to Login due to Internal Error. Please try again.";
                    return View("Error");
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
                        To = model.Email,
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
            }

			catch(Exception e)
            {
               throw(e);
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
			var enPassword = model.Password;
			EncryptDecrypt objEncryptDecrypt1 = new EncryptDecrypt();
			enPassword = objEncryptDecrypt1.Encrypt(model.Password, WebConfigurationManager.AppSettings["ServiceAccountPassword"]);

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
                    areAllAnswersValid = sq.Value == item.Value;
                });
            }

            if(areAllAnswersValid)
            {
				ResetPasswordModel rpm = new ResetPasswordModel
				{
					Email = model.Email,
					Password = enPassword
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
            Session.Clear();
            Session.Abandon();
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
		public async Task<ActionResult> PersonalDetails()
		{
            ViewBag.Gender = await _common.GetGenderData();
			ViewBag.AgeGroup = await _common.GetAgeGroupData();

			if (TempData["PersonalDetails"] != null)
			{
				var pd = TempData["PersonalDetails"] as PersonalDetails;
				return View(pd);
			}
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
			ViewBag.SelectedCountry = await _account.GetCountryId("United States");

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
		[AllowAnonymous]
		public async Task<ActionResult> ContactDetails()
        {
			ViewBag.CountryList = await _common.GetCountryData();
			ViewBag.SelectedCountry = await _account.GetCountryId("United States");
			if (TempData["ContactDetails"] != null)
			{
				var cd = TempData["ContactDetails"] as ContactDetails;
				ViewBag.SelectedCountry = cd.Country;
				ViewBag.SelectedState = cd.State;
				return View(cd);
			}
			return View();
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
			ViewBag.SelectedCountry = await _account.GetCountryId("United States");

			if (prevBtn != null)
			{
				PersonalDetails pd = new PersonalDetails();
				pd.FirstName = obj.FirstName;
				pd.LastName = obj.LastName;
				pd.DOB = obj.DOB;
				pd.GenderData = obj.GenderId;
				if (obj.AgeGroupId == null)
				{
					pd.AgeGroupData = null;
					ViewBag.SelectedAgeGroup = null;
				}
				else
				{
					pd.AgeGroupData = (int)obj.AgeGroupId;
					ViewBag.SelectedAgeGroup = (int)obj.AgeGroupId;
				}
				ViewBag.SelectedGender = obj.GenderId;
				TempData["PersonalDetails"] = pd;
                return RedirectToAction("PersonalDetails");
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
		[AllowAnonymous]
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
            ViewBag.SelectedCountry = obj.CountryId;
            cd.State = obj.StateId;
            ViewBag.SelectedState = obj.StateId;
            cd.City = obj.City;
            cd.ZipCode = obj.ZipCode;
            cd.HomePhone = obj.HomePhone;
            cd.CellPhone = obj.CellPhone;

            if (prevBtn != null)
			{
				TempData["ContactDetails"] = cd;
				return RedirectToAction("ContactDetails");
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
				ViewBag.SecurityQuestions = await _common.GetSecurityQuestions();
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
                        bool isEmailExists = await _account.IsActiveUser(data.Email);
                        bool isFamilyMember = await _account.IsFamilyMember(data.Email);
						int isAddressOrHomePhoneMatched = await _account.IsAddressOrHomePhoneMatched(cd);
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
                        if (isFamilyMember || isAddressOrHomePhoneMatched != 0)
                        {
                            obj.IsIndividual = true;
                            obj.IsApproveMailSent = true;
							string primaryAccountEmail = string.Empty;

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
                            string toUserFullname = string.IsNullOrEmpty(um.Id) ? obj.FirstName + " " + obj.LastName : await _user.GetUserFullName(data.Email);
							if (isAddressOrHomePhoneMatched == 0)
							{
								primaryAccountEmail = await _account.GetFamilyPrimaryAccountEmail(data.Email);
							}
							else {
								primaryAccountEmail =
								isAddressOrHomePhoneMatched == 1
								? await _account.GetPrimaryAccountEmailByHomePhone(obj.HomePhone)
								: await _account.GetPrimaryAccountEmailByAddress(cd);
							}
                                                                               
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
								To = primaryAccountEmail,
								Subject = etm1.Subject,
                                From = ConfigurationManager.AppSettings["SMTPUsername"]
                            };
                            em1.Send();

                            obj.Id = null;
                            ViewBag.ApproveContent = "An approval email has been sent to primary account holder of your family..! Your account will be activated once your request has been approved.";
                            if (!userRejected)
                            {
                                HttpResponseMessage userResponseMessage = await Utility.GetObject("/api/User/PostUser", obj, true);

								SharedAccountModel sam = new SharedAccountModel();
								sam.To_UserId = await _account.GetUserIdByEmail(primaryAccountEmail);
								sam.From_UserId = await _account.GetUserIdByEmail(obj.Email);
								sam.CreatedDate = DateTime.Now;
								HttpResponseMessage userResponseMessage2 = await Utility.GetObject("/api/User/PostSharedAccount", sam, true);
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
                                To = data.Email,
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
				um.Status = true;
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
			um.EmailConfirmed = um.Status;

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
                    To = arm.Email,
                    Subject = etm.Subject,
                    From = ConfigurationManager.AppSettings["SMTPUsername"]
                };
                em.Send();

				SharedAccountModel sam = new SharedAccountModel();
				sam.From_UserId = await _account.GetUserIdByEmail(um.Email);
				if (status == "Approved")
					sam.IsApproved = true;
				else
					sam.IsDeclined = true;
				sam.UpdatedDate = DateTime.Now;
				HttpResponseMessage userResponseMessage2 = await Utility.GetObject("/api/User/UpdateSharedAccount", sam, true);
				var user = await Utility.DeserializeObject<SharedAccountModel>(userResponseMessage2);


				if (arm.AreAddressDetailsMatched && arm.IsApproved)
                {
                    FamilyMemberModel fm = new FamilyMemberModel();
                    fm.CellPhone = um.CellPhone;
                    fm.DOB = um.DOB;
                    fm.Email = um.Email;
                    fm.FirstName = um.FirstName;
                    fm.GenderData = um.GenderId;
                    fm.LastName = um.LastName;
                    fm.RelationshipData = 6;
					fm.MonthlyNewsLetter = false;
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
		[CustomAuthorize(Roles = "Admin,User")]
		public async Task<ActionResult> MyAccount(ToastModel tm = null)
		{
			
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
	}
}