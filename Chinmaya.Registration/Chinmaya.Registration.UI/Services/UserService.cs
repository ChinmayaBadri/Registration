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
            string urlAction = "api/Account/GetUserInfoByEmail/" + email + "/";
            HttpResponseMessage getUserInfoResponse = await Utility.GetObject(urlAction);

            return await Utility.DeserializeObject<UserModel>(getUserInfoResponse);
        }

        public async Task<string> GetUserIdByEmail(string email)
        {
            string urlAction = "api/Account/GetUserIdByEmail/" + email + "/";
            HttpResponseMessage getUserIDResponse = await Utility.GetObject(urlAction);

            return await Utility.DeserializeObject<string>(getUserIDResponse);
        }

        public async Task<List<UserFamilyMember>> GetUserFamilyMemberData(string Id)
        {
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/UserAPI/GetUserFamilyMemberData/" + Id, true);
            return await Utility.DeserializeObject<List<UserFamilyMember>>(roleResponseMessage);
        }

        public async Task<FamilyMemberModel> FamilyMemberDetails(string Id)
        {
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/UserAPI/GetFamilyMemberDetails/" + Id, true);
            return await Utility.DeserializeObject<FamilyMemberModel>(roleResponseMessage);
        }

        public async Task<bool> IsFamilyMember(string email)
        {
            string urlAction = "api/Account/IsFamilyMember/" + email + "/";
            HttpResponseMessage isEmailExistResponse = await Utility.GetObject(urlAction);
            return await Utility.DeserializeObject<bool>(isEmailExistResponse);
        }

        public async Task<UserFamilyMember> GetUserData(string Id)
        {
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/UserAPI/GetUserData/" + Id, true);
            return await Utility.DeserializeObject<UserFamilyMember>(roleResponseMessage);
        }
    }
}