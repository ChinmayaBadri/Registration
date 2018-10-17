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
