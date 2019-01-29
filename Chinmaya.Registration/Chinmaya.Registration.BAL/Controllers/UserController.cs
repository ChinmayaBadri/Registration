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

        /// <summary>
        /// gets user info by email
        /// </summary>
        /// <param name="entity"> Login View Model </param>
        /// <returns> User model </returns>
        [ResponseType(typeof(UserModel))]
        public IHttpActionResult Post(LoginViewModel entity)
        {
            return Ok(_user.GetUserInfo(entity));
        }

        /// <summary>
        /// gets user role name by id
        /// </summary>
        /// <param name="id"> user id </param>
        /// <returns> role name </returns>
        public string Get(int id)
        {
            return _user.GetRoleName(id);
        }

        /// <summary>
        /// gets user family members data by user id
        /// </summary>
        /// <param name="Id"> user id </param>
        /// <returns> User Family member model </returns>
		[Route("api/User/GetUserFamilyMemberData/{id}")]
		[HttpGet]
		public IEnumerable<GetFamilyMemberForUser_Result> GetUserFamilyMemberData(string id)
		{
			return _user.GetUserFamilyMemberData(id);
		}

        /// <summary>
        /// gets user data
        /// </summary>
        /// <param name="Id"> user id </param>
        /// <returns>User Family Member model </returns>
		[Route("api/User/GetUserData/{id}")]
		[HttpGet]
		public UserFamilyMember GetUserData(string id)
		{
			return _user.GetUserData(id);
		}

        /// <summary>
        /// gets all users details
        /// </summary>
        /// <returns> list of users object </returns>
		[Route("api/User/GetAllUsers")]
		[HttpGet]
		public object GetAllUsers()
		{
			return _user.GetAllUsers();
		}

        /// <summary>
        /// gets all family members details
        /// </summary>
        /// <param name="id"> user id </param>
        /// <returns> list of family members object </returns>
		[Route("api/User/GetAllFamilyMembers/{id}")]
		[HttpGet]
		public object GetAllFamilyMembers(string id)
		{
			return _user.GetAllFamilyMembers(id);
		}

        /// <summary>
        /// add or update user data
        /// </summary>
        /// <param name="user"> User Model </param>
		[Route("api/User/PostUser")]
		[HttpPost]
		public IHttpActionResult PostUser(UserModel obj)
		{
			try
			{
				_user.PostUser(obj);
				return Ok("Success");
			}
			catch
			{
				throw;
			}
		}

        /// <summary>
        /// add or edit family member details
        /// </summary>
        /// <param name="family"> Family Member Model </param>
		[Route("api/User/PostFamilyMember")]
		[HttpPost]
		public IHttpActionResult PostFamilyMember(FamilyMemberModel obj)
		{
			try
			{
				_user.PostFamilyMember(obj);
				return Ok("Success");
			}
			catch
			{
				throw;
			}
		}

        /// <summary>
        /// get family member details
        /// </summary>
        /// <param name="Id"> family member id </param>
        /// <returns> family member model </returns>
		[Route("api/User/GetFamilyMemberDetails/{id}")]
		[HttpGet]
		public FamilyMemberModel GetFamilyMemberDetails(string id)
		{
			return _user.GetFamilyMemberDetails(id);
		}

        /// <summary>
        /// gets user full name by email
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> user full name </returns>
        [Route("api/User/GetUserFullNameByEmail/{email}/")]
        [HttpGet]
        public IHttpActionResult GetUserFullNameByEmail(string email)
        {
            return Ok(_user.GetUserFullNameByEmail(email));
        }

        /// <summary>
        /// get user info by email address
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> User Model </returns>
        [Route("api/User/GetUserInfoByEmail/{email}/")]
        [HttpGet]
        public IHttpActionResult GetUserInfoByEmail(string email)
        {
            return Ok(_user.GetUserInfoByEmail(email));
        }

        /// <summary>
        /// gets User phone no.
        /// </summary>
        /// <param name="Email"> user email </param>
        /// <returns> Update Phone model </returns>
		[Route("api/User/getPhoneNumber/{email}/")]
		[HttpGet]
		public UpdatePhone getPhoneNumber(string email)
		{
			return _user.getPhoneNumber(email);
		}

        /// <summary>
        /// gets user email
        /// </summary>
        /// <param name="Email"> user email </param>
        /// <returns> Update email model  </returns>
		[Route("api/User/getEmail/{email}/")]
		[HttpGet]
		public UpdateEmail getEmail(string email)
		{
			return _user.getEmail(email);
		}

        /// <summary>
        /// gets user address
        /// </summary>
        /// <param name="Email"> user email </param>
        /// <returns> Contact Details model </returns>
		[Route("api/User/getAddress/{email}/")]
		[HttpGet]
		public ContactDetails getAddress(string email)
		{
			return _user.getAddress(email);
		}

		/// <summary>
		/// updates the user password
		/// </summary>
		/// <param name="obj">Update Password Model</param>
		[Route("api/User/UpdatePassword")]
		[HttpPost]
		public bool UpdatePassword(UpdatePasswordModel obj)
		{
			try
			{
				return _user.UpdatePassword(obj);				
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Updates Phone number of User
		/// </summary>
		/// <param name="obj">Update Phone Model</param>
		[Route("api/User/UpdatePhone")]
		[HttpPost]
		public IHttpActionResult UpdatePhone(UpdatePhone obj)
		{
			try
			{
				_user.UpdatePhone(obj);
				return Ok("Success");
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Update Email Address of User
		/// </summary>
		/// <param name="obj">Update Email Model</param>
		[Route("api/User/UpdateEmailAddress")]
		[HttpPost]
		public IHttpActionResult UpdateEmailAddress(UpdateEmail obj)
		{
			try
			{
				_user.UpdateEmailAddress(obj);
				return Ok("Success");
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Updates Address of User
		/// </summary>
		/// <param name="obj">Contact Deatils Model</param>
		/// <returns></returns>
		[Route("api/User/UpdateAddress")]
		[HttpPost]
		public IHttpActionResult UpdateAddress(ContactDetails obj)
		{
			try
			{
				_user.UpdateAddress(obj);
				return Ok("Success");
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// gets user role name by email
		/// </summary>
		/// <param name="email"> user email </param>
		/// <returns> user role name </returns>
		[Route("api/User/GetUserRoleNameByEmail/{email}/")]
		[HttpGet]
		public IHttpActionResult GetUserRoleNameByEmail(string email)
		{
			return Ok(_user.GetUserRoleNameByEmail(email));
		}

		/// <summary>
		/// adds shareduser data
		/// </summary>
		/// <param name="obj"> SharedAccount Model </param>
		[Route("api/User/PostSharedAccount")]
		[HttpPost]
		public IHttpActionResult PostSharedAccount(SharedAccountModel obj)
		{
			try
			{
				_user.PostSharedAccount(obj);
				return Ok("Success");
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// updates shareduser data
		/// </summary>
		/// <param name="obj"> SharedAccount Model </param>
		[Route("api/User/UpdateSharedAccount")]
		[HttpPost]
		public SharedAccount UpdateSharedAccount(SharedAccountModel obj)
		{
			try
			{
				return _user.UpdateSharedAccount(obj);
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// get admin info
		/// </summary>
		/// <returns> User Model </returns>
		[Route("api/User/GetAdminInfo/")]
		[HttpGet]
		public IHttpActionResult GetAdminInfo()
		{
			return Ok(_user.GetAdminInfo());
		}

	}
}
