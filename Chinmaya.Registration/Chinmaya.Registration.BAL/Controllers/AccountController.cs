using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Chinmaya.Registration.DAL;
using Chinmaya.Registration.Models;

namespace Chinmaya.Registration.BAL.Controllers
{
    public class AccountController : ApiController
    {
		Account _Account = new Account();
        Common _common = new Common();

        /// <summary>
        /// gets states
        /// </summary>
        /// <param name="id"> country id </param>
        /// <returns> list of states </returns>
		[Route("api/Account/GetState/{id}")]
		[HttpGet]
		public List<KeyValueModel> GetState(int id)
		{
			return _common.GetStateName(id);
		}

        /// <summary>
        /// gets cities
        /// </summary>
        /// <param name="id"> state id </param>
        /// <returns> list of cities </returns>
		[Route("api/Account/GetCity/{id}")]
		[HttpGet]
		public List<KeyValueModel> GetCity(int id)
		{
			return _common.GetCityName(id);
		}

        /// <summary>
        /// gets country id
        /// </summary>
        /// <param name="name"> country name </param>
        /// <returns> country id </returns>
		[Route("api/Account/GetCountryId/{name}/")]
		[HttpGet]
		public int GetCountryId(string name)
		{
			return _common.GetCountryId(name);
		}

        /// <summary>
        /// Check user email exists or not
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> true or false </returns>
		[Route("api/Account/IsEmailExists/{email}/")]
        [HttpGet]
        public bool IsEmailExists(string email)
        {
            return _Account.IsEmailExists(email);
        }

        /// <summary>
        /// checks user addess or home phone is matched with any other account's address or home phone
        /// </summary>
        /// <param name="cd"> contact details model </param>
        /// <returns> true or false </returns>
        [Route("api/Account/AreAddressDetailsMatched")]
        [HttpPost]
        public bool AreAddressDetailsMatched(ContactDetails cd)
        {
            return _Account.AreAddressDetailsMatched(cd);
        }

        /// <summary>
        /// gets email template by template id
        /// </summary>
        /// <param name="id"> template id </param>
        /// <returns> email template model </returns>
        [Route("api/Account/GetEmailTemplateByID/{id}")]
        [HttpGet]
        public IHttpActionResult GetEmailTemplate(int id)
        {
            return Ok(_common.GetEmailTemplateByID(id));
        }

        /// <summary>
        /// Gets security questions by email
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> list of security questions model </returns>
        [Route("api/Account/GetSecurityQuestionsByEmail/{email}/")]
        [HttpGet]
        public IHttpActionResult GetSecurityQuestionsByEmail(string email)
        {
            return Ok(_Account.GetSecurityQuestionsByEmail(email));
        }

        /// <summary>
        /// Get family primary account email id
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> primary account email id </returns>
        [Route("api/Account/GetFamilyPrimaryAccountEmail/{email}/")]
        [HttpGet]
        public IHttpActionResult GetFamilyPrimaryAccountEmail(string email)
        {
            return Ok(_Account.GetFamilyPrimaryAccountEmail(email));
        }

        /// <summary>
        /// update user password
        /// </summary>
        /// <param name="rpm">Reset Password Model</param>
        /// <returns> true or false </returns>
        [Route("api/Account/ResetUserPassword")]
        [HttpPost]
        public IHttpActionResult ResetUserPassword(ResetPasswordModel rpm)
        {
            return Ok(_Account.ResetUserPassword(rpm));
        }

        /// <summary>
        /// checks is user family member or not
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> true or false </returns>
        [Route("api/Account/IsFamilyMember/{email}/")]
        [HttpGet]
        public IHttpActionResult IsFamilyMember(string email)
        {
            return Ok(_Account.IsFamilyMember(email));
        }

        /// <summary>
        /// checks whether user is active or inactive user
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> true or false </returns>
        [Route("api/Account/IsActiveUser/{email}/")]
        [HttpGet]
        public IHttpActionResult IsActiveUser(string email)
        {
            return Ok(_Account.IsActiveUser(email));
        }

        /// <summary>
        /// gets user id by email
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> User id </returns>
        [Route("api/Account/GetUserIdByEmail/{email}/")]
        [HttpGet]
        public IHttpActionResult GetUserIdByEmail(string email)
        {
            return Ok(_Account.GetUserIdByEmail(email));
        }

        /// <summary>
        /// user account type will changed
        /// </summary>
        /// <param name="id"> user id </param>
        [Route("api/Account/ChangeAccountType/{id}")]
        [HttpPost]
        public IHttpActionResult ChangeAccountType(string id)
        {
            try
            {
                _Account.ChangeAccountType(id);
                return Ok("Success");
            }
            catch (Exception)
            {
                return Ok("Something went wrong");
            }
        }

        /// <summary>
        /// gets user account type
        /// </summary>
        /// <param name="Id"> user id </param>
        /// <returns> true or false </returns>
        [Route("api/Account/GetIsIndividual/{id}")]
        [HttpGet]
        public bool GetIsIndividual(string id)
        {
            return _Account.GetIsIndividual(id);
        }
    }
}
