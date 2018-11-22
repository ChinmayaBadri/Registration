using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Chinmaya.Registration.DAL;
using Chinmaya.Registration.Models;
using System.Web.Http.Description;
using Chinmaya.DAL;
using Chinmaya.Models;

namespace Chinmaya.Registration.BAL.Controllers
{
    public class UserAPIController : ApiController
    {
        Users _user = new Users();


        [ResponseType(typeof(UserModel))]
        public IHttpActionResult Post(LoginViewModel entity)
        {
            return Ok(_user.GetUserInfo(entity));
        }

        public string Get(int id)
        {
            return _user.GetRoleName(id);
        }

		[Route("api/UserAPI/GetUserFamilyMemberData/{id}")]
		[HttpGet]
		public IEnumerable<GetFamilyMemberForUser_Result> GetUserFamilyMemberData(string id)
		{
			return _user.GetUserFamilyMemberData(id);
		}

		//[Route("api/UserAPI/GetFamilyMemberDetails/{id}")]
		//[HttpGet]
		//public FamilyMemberModel GetFamilyMemberDetails(string id)
		//{
		//	return _user.GetFamilyMemberDetails(id);
		//}

		[Route("api/UserAPI/GetUserData/{id}")]
		[HttpGet]
		public UserFamilyMember GetUserData(string id)
		{
			return _user.GetUserData(id);
		}

		[Route("api/UserAPI/GetIsIndividual/{id}")]
		[HttpGet]
		public bool GetIsIndividual(string id)
		{
			return _user.GetIsIndividual(id);
		}

		[Route("api/UserAPI/GetEventData/{id}")]
		[HttpGet]
		public CurrentEventModel GetEventData(string id)
		{
			return _user.GetEventData(id);
		}

		[Route("api/UserAPI/GetEventsData/")]
		[HttpGet]
		public IEnumerable<CurrentEventModel> GetEventsData()
		{
			return _user.GetEventsData();
		}

		//[Route("api/UserAPI/GetEventsListData/{id}")]
		//[HttpGet]
		//public IEnumerable<ProgramEventRegistrationModel> GetEventsListData(string id)
		//{
		//	return _user.GetEventsListData(id);
		//}

		[Route("api/UserAPI/PostUser")]
		[HttpPost]
		public IHttpActionResult PostUser(UserModel obj)
		{
			try
			{
				_user.PostUser(obj);
				return Ok("Success");
			}
			catch (Exception)
			{
				return Ok("Something went wrong");
			}
		}

		[Route("api/UserAPI/PostFamilyMember")]
		[HttpPost]
		public IHttpActionResult PostFamilyMember(FamilyMemberModel obj)
		{
			try
			{
				_user.PostFamilyMember(obj);
				return Ok("Success");
			}
			catch (Exception)
			{
				return Ok("Something went wrong");
			}
		}

		[Route("api/UserAPI/PostEvent")]
		[HttpPost]
		public IHttpActionResult PostEvent(EventsModel obj)
		{
			try
			{
				_user.PostEvent(obj);
				return Ok("Success");
			}
			catch (Exception)
			{
				return Ok("Something went wrong");
			}
		}

		[Route("api/UserAPI/PostCheckPayment")]
		[HttpPost]
		public IHttpActionResult PostCheckPayment(CheckPaymentModel obj)
		{
			try
			{
				_user.PostCheckPayment(obj);
				return Ok("Success");
			}
			catch (Exception)
			{
				return Ok("Something went wrong");
			}
		}

		//// GET: api/User
		//public IEnumerable<string> Get()
		//{
		//    return _user.GetUsers();
		//}

		//// GET: api/User/5
		//public string Get(int id)
		//{
		//    return _user.GetUserName(id);
		//}

		//// POST: api/User
		//public void Post([FromBody]string value)
		//{
		//}

		//// PUT: api/User/5
		//public void Put(int id, [FromBody]string value)
		//{
		//}

		//// DELETE: api/User/5
		//public void Delete(int id)
		//{
		//}	
	}
}
