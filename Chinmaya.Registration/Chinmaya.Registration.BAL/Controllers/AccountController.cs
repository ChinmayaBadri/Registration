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

		// GET: api/Account
		public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Account/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Account
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Account/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Account/5
        public void Delete(int id)
        {
        }

		[Route("api/Account/GetState/{id}")]
		[HttpGet]
		public List<KeyValueModel> GetState(int id)
		{
			return _Account.GetStateName(id);
		}

		[Route("api/Account/GetCity/{id}")]
		[HttpGet]
		public List<KeyValueModel> GetCity(int id)
		{
			return _Account.GetCityName(id);
		}

        [Route("api/Account/IsEmailExists/{email}/")]
        [HttpGet]
        public bool IsEmailExists(string email)
        {
            return _Account.IsEmailExists(email);
        }

        [Route("api/Account/AreAddressDetailsMatched")]
        [HttpPost]
        public bool AreAddressDetailsMatched(ContactDetails cd)
        {
            return _Account.AreAddressDetailsMatched(cd);
        }

        [Route("api/Account/GetEmailTemplateByID/{id}")]
        [HttpGet]
        public IHttpActionResult GetEmailTemplate(int id)
        {
            return Ok(_Account.GetEmailTemplateByID(id));
        }

        [Route("api/Account/GetSecurityQuestionsByEmail/{email}/")]
        [HttpGet]
        public IHttpActionResult GetSecurityQuestionsByEmail(string email)
        {
            return Ok(_Account.GetSecurityQuestionsByEmail(email));
        }

        [Route("api/Account/GetFamilyPrimaryAccountEmail/{email}/")]
        [HttpGet]
        public IHttpActionResult GetFamilyPrimaryAccountEmail(string email)
        {
            return Ok(_Account.GetFamilyPrimaryAccountEmail(email));
        }

        [Route("api/Account/GetUserFullNameByEmail/{email}/")]
        [HttpGet]
        public IHttpActionResult GetUserFullNameByEmail(string email)
        {
            return Ok(_Account.GetUserFullNameByEmail(email));
        }

        [Route("api/Account/ResetUserPassword")]
        [HttpPost]
        public IHttpActionResult ResetUserPassword(ResetPasswordModel rpm)
        {
            return Ok(_Account.ResetUserPassword(rpm));
        }

        [Route("api/Account/IsFamilyMember/{email}/")]
        [HttpGet]
        public IHttpActionResult IsFamilyMember(string email)
        {
            return Ok(_Account.IsFamilyMember(email));
        }

        [Route("api/Account/GetUserInfoByEmail/{email}/")]
        [HttpGet]
        public IHttpActionResult GetUserInfoByEmail(string email)
        {
            return Ok(_Account.GetUserInfoByEmail(email));
        }

        [Route("api/Account/IsActiveUser/{email}/")]
        [HttpGet]
        public IHttpActionResult IsActiveUser(string email)
        {
            return Ok(_Account.IsActiveUser(email));
        }

        [Route("api/Account/GetUserIdByEmail/{email}/")]
        [HttpGet]
        public IHttpActionResult GetUserIdByEmail(string email)
        {
            return Ok(_Account.GetUserIdByEmail(email));
        }

        /*[Route("Api/Account/Gender")]
		//GET: api/Account/Gender
		[ActionName("Gender")]
		public List<KeyValueModel> GetGender()
		{
			return _Account.GetGender();
		}

		[Route("Api/Account/Country")]
		//GET: api/Account/Country
		[ActionName("Country")]
		public List<KeyValueModel> GetCountryList()
		{
			return _Account.GetCountryList();
		}

		[Route("Api/Account/State/{Id}")]
		//GET: api/UserAPI/State/{id}
		[ActionName("State")]
		public List<KeyValueModel> FillState(int id)
		{
			return _Account.GetStateList(id);
		}

		[Route("Api/Account/City/{id?}")]
		//GET: api/UserAPI/City/{id}
		[ActionName("City")]
		public List<KeyValueModel> FillCity(int id)
		{
			return _Account.GetCityList(id);
		}*/
    }
}
