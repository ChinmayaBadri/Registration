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
    public class UserController : ApiController
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

		[Route("api/User/GetUserFamilyMemberData/{id}")]
		[HttpGet]
		public IEnumerable<GetFamilyMemberForUser_Result> GetUserFamilyMemberData(string id)
		{
			return _user.GetUserFamilyMemberData(id);
		}

		[Route("api/User/GetUserData/{id}")]
		[HttpGet]
		public UserFamilyMember GetUserData(string id)
		{
			return _user.GetUserData(id);
		}

		[Route("api/User/GetAllUsers")]
		[HttpGet]
		public object GetAllUsers()
		{
			return _user.GetAllUsers();
		}

		[Route("api/User/GetAllFamilyMembers/{id}")]
		[HttpGet]
		public object GetAllFamilyMembers(string id)
		{
			return _user.GetAllFamilyMembers(id);
		}

		[Route("api/User/PostUser")]
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

		[Route("api/User/PostFamilyMember")]
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

		[Route("api/User/GetFamilyMemberDetails/{id}")]
		[HttpGet]
		public FamilyMemberModel GetFamilyMemberDetails(string id)
		{
			return _user.GetFamilyMemberDetails(id);
		}

        [Route("api/User/GetUserFullNameByEmail/{email}/")]
        [HttpGet]
        public IHttpActionResult GetUserFullNameByEmail(string email)
        {
            return Ok(_user.GetUserFullNameByEmail(email));
        }

        [Route("api/User/GetUserInfoByEmail/{email}/")]
        [HttpGet]
        public IHttpActionResult GetUserInfoByEmail(string email)
        {
            return Ok(_user.GetUserInfoByEmail(email));
        }
    }
}
