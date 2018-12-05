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
        public async Task<UserModel> GetUserInfo(string email)
        {
            string urlAction = "api/User/GetUserInfoByEmail/" + email + "/";
            HttpResponseMessage getUserInfoResponse = await Utility.GetObject(urlAction);

            return await Utility.DeserializeObject<UserModel>(getUserInfoResponse);
        }

        public async Task<List<UserFamilyMember>> GetUserFamilyMemberData(string Id)
        {
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/User/GetUserFamilyMemberData/" + Id, true);
            return await Utility.DeserializeObject<List<UserFamilyMember>>(roleResponseMessage);
        }

        public async Task<FamilyMemberModel> FamilyMemberDetails(string Id)
        {
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/User/GetFamilyMemberDetails/" + Id, true);
            return await Utility.DeserializeObject<FamilyMemberModel>(roleResponseMessage);
        }

		public async Task<UpdatePhone> getPhoneNumber(string email)
		{
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/User/getPhoneNumber/" + email + "/", true);
			return await Utility.DeserializeObject<UpdatePhone>(roleResponseMessage);
		}

		public async Task<ContactDetails> getAddress(string email)
		{
			HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/User/getAddress/" + email + "/", true);
			return await Utility.DeserializeObject<ContactDetails>(roleResponseMessage);
		}

        public async Task<UserFamilyMember> GetUserData(string Id)
        {
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/User/GetUserData/" + Id, true);
            return await Utility.DeserializeObject<UserFamilyMember>(roleResponseMessage);
        }

        public async Task<string> GetUserFullName(string email)
        {
            string urlAction = "api/User/GetUserFullNameByEmail/" + email + "/";
            HttpResponseMessage getFullnameResponse = await Utility.GetObject(urlAction);

            return await Utility.DeserializeObject<string>(getFullnameResponse);
        }
    }
}