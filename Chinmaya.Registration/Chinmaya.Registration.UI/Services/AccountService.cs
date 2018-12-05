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
    public class AccountService
    {
		public async Task<int> GetCountryId(string name)
		{
			string urlAction = "api/Account/GetCountryId/" + name;
			HttpResponseMessage getFullnameResponse = await Utility.GetObject(urlAction);

			return await Utility.DeserializeObject<int>(getFullnameResponse);
		}

		public async Task<EmailTemplateModel> GetEmailTemplate(int id)
        {
            string urlAction = "api/Account/GetEmailTemplateByID/" + id;

            HttpResponseMessage emailTemplateResponse = await Utility.GetObject(urlAction);
            return await Utility.DeserializeObject<EmailTemplateModel>(emailTemplateResponse);
        }


        public async Task<string> GetFamilyPrimaryAccountEmail(string email)
        {
            string urlAction = "api/Account/GetFamilyPrimaryAccountEmail/" + email + "/";
            HttpResponseMessage getPrimaryEmailResponse = await Utility.GetObject(urlAction);

            return await Utility.DeserializeObject<string>(getPrimaryEmailResponse);
        }

        public async Task<bool> IsAddressOrHomePhoneMatched(ContactDetails cd)
        {
            string urlAction = "api/Account/AreAddressDetailsMatched";
            HttpResponseMessage areDetailsMatchedRes = await Utility.GetObject(urlAction, cd, true);

            return await Utility.DeserializeObject<bool>(areDetailsMatchedRes);
        }

        public async Task<List<SecurityQuestionsModel>> GetSecurityQuestionsByEmail(string email)
        {
            string urlAction2 = "api/Account/GetSecurityQuestionsByEmail/" + email + "/";
            HttpResponseMessage getSecurityQuestionsRes = await Utility.GetObject(urlAction2);

            return await Utility.DeserializeList<SecurityQuestionsModel>(getSecurityQuestionsRes);
        }

        public async Task<bool> GetIsIndividual(string Id)
        {
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/Account/GetIsIndividual/" + Id, true);
            return await Utility.DeserializeObject<bool>(roleResponseMessage);
        }

        public async Task<bool> CheckIsEmailExists(string email)
        {
            string urlAction = "api/Account/IsEmailExists/" + email + "/";
            HttpResponseMessage isEmailExistResponse = await Utility.GetObject(urlAction);
            return await Utility.DeserializeObject<bool>(isEmailExistResponse);
        }

        public async Task<string> GetUserIdByEmail(string email)
        {
            string urlAction = "api/Account/GetUserIdByEmail/" + email + "/";
            HttpResponseMessage getUserIDResponse = await Utility.GetObject(urlAction);

            return await Utility.DeserializeObject<string>(getUserIDResponse);
        }

        public async Task<bool> IsFamilyMember(string email)
        {
            string urlAction = "api/Account/IsFamilyMember/" + email + "/";
            HttpResponseMessage isEmailExistResponse = await Utility.GetObject(urlAction);
            return await Utility.DeserializeObject<bool>(isEmailExistResponse);
        }
    }
}