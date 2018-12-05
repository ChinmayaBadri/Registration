using Chinmaya.Registration.Models;
using Chinmaya.Registration.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Chinmaya.Registration.UI.Services
{
    public class UserService
    {
        /// <summary>
        /// gets user info by email
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> User model </returns>
        public async Task<UserModel> GetUserInfo(string email)
        {
            string urlAction = "api/User/GetUserInfoByEmail/" + email + "/";
            HttpResponseMessage getUserInfoResponse = await Utility.GetObject(urlAction);

            return await Utility.DeserializeObject<UserModel>(getUserInfoResponse);
        }

        /// <summary>
        /// gets user family members data by user id
        /// </summary>
        /// <param name="Id"> user id </param>
        /// <returns> User Family member model </returns>
        public async Task<List<UserFamilyMember>> GetUserFamilyMemberData(string Id)
        {
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/User/GetUserFamilyMemberData/" + Id, true);
            return await Utility.DeserializeObject<List<UserFamilyMember>>(roleResponseMessage);
        }

        /// <summary>
        /// get family member details
        /// </summary>
        /// <param name="Id"> family member id </param>
        /// <returns> family member details </returns>

        public async Task<FamilyMemberModel> FamilyMemberDetails(string Id)
        {
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/User/GetFamilyMemberDetails/" + Id, true);
            return await Utility.DeserializeObject<FamilyMemberModel>(roleResponseMessage);
        }

        /// <summary>
        /// gets user phone number by email id
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> update phone model </returns>

		public async Task<UpdatePhone> getPhoneNumber(string email)
		{
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/User/getPhoneNumber/" + email + "/", true);
			return await Utility.DeserializeObject<UpdatePhone>(roleResponseMessage);
		}

        /// <summary>
        /// gets user address by email
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> Contact details model </returns>
		public async Task<ContactDetails> getAddress(string email)
		{
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/User/getAddress/" + email + "/", true);
			return await Utility.DeserializeObject<ContactDetails>(roleResponseMessage);
		}

        /// <summary>
        /// gets user data by userId
        /// </summary>
        /// <param name="Id"> user Id</param>
        /// <returns> User Family Member model</returns>
        public async Task<UserFamilyMember> GetUserData(string Id)
        {
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/User/GetUserData/" + Id, true);
            return await Utility.DeserializeObject<UserFamilyMember>(roleResponseMessage);
        }

        /// <summary>
        /// gets user full name by email
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> user full name </returns>
        public async Task<string> GetUserFullName(string email)
        {
            string urlAction = "api/User/GetUserFullNameByEmail/" + email + "/";
            HttpResponseMessage getFullnameResponse = await Utility.GetObject(urlAction);

            return await Utility.DeserializeObject<string>(getFullnameResponse);
        }
    }
}