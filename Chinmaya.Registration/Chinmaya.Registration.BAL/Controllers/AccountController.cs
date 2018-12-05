﻿using System;
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

		[Route("api/Account/GetState/{id}")]
		[HttpGet]
		public List<KeyValueModel> GetState(int id)
		{
			return _common.GetStateName(id);
		}

		[Route("api/Account/GetCity/{id}")]
		[HttpGet]
		public List<KeyValueModel> GetCity(int id)
		{
			return _common.GetCityName(id);
		}

		[Route("api/Account/GetCountryId/{name}")]
		[HttpGet]
		public int GetCountryId(string name)
		{
			return _common.GetCountryId(name);
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
            return Ok(_common.GetEmailTemplateByID(id));
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

        [Route("api/Account/GetIsIndividual/{id}")]
        [HttpGet]
        public bool GetIsIndividual(string id)
        {
            return _Account.GetIsIndividual(id);
        }
    }
}
