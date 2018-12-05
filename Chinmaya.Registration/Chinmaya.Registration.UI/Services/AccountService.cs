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
        /// <summary>
        /// gets country Id
        /// </summary>
        /// <param name="name"> country name </param>
        /// <returns> country id </returns>
		public async Task<int> GetCountryId(string name)
		{
			string urlAction = "api/Account/GetCountryId/" + name;
			HttpResponseMessage getFullnameResponse = await Utility.GetObject(urlAction);

			return await Utility.DeserializeObject<int>(getFullnameResponse);
		}

        /// <summary>
        /// gets email template
        /// </summary>
        /// <param name="id"> template id </param>
        /// <returns> Email template model </returns>
		public async Task<EmailTemplateModel> GetEmailTemplate(int id)
        {
            string urlAction = "api/Account/GetEmailTemplateByID/" + id;

            HttpResponseMessage emailTemplateResponse = await Utility.GetObject(urlAction);
            return await Utility.DeserializeObject<EmailTemplateModel>(emailTemplateResponse);
        }

        /// <summary>
        /// Get family primary account email id
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> primary account email id </returns>
        public async Task<string> GetFamilyPrimaryAccountEmail(string email)
        {
            string urlAction = "api/Account/GetFamilyPrimaryAccountEmail/" + email + "/";
            HttpResponseMessage getPrimaryEmailResponse = await Utility.GetObject(urlAction);

            return await Utility.DeserializeObject<string>(getPrimaryEmailResponse);
        }

        /// <summary>
        /// checks user addess or home phone is matched with any other account's address or home phone
        /// </summary>
        /// <param name="cd"> contact details model </param>
        /// <returns> true or false </returns>
        public async Task<bool> IsAddressOrHomePhoneMatched(ContactDetails cd)
        {
            string urlAction = "api/Account/AreAddressDetailsMatched";
            HttpResponseMessage areDetailsMatchedRes = await Utility.GetObject(urlAction, cd, true);

            return await Utility.DeserializeObject<bool>(areDetailsMatchedRes);
        }

        /// <summary>
        /// Gets security questions by email
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> list of security questions model </returns>
        public async Task<List<SecurityQuestionsModel>> GetSecurityQuestionsByEmail(string email)
        {
            string urlAction2 = "api/Account/GetSecurityQuestionsByEmail/" + email + "/";
            HttpResponseMessage getSecurityQuestionsRes = await Utility.GetObject(urlAction2);

            return await Utility.DeserializeList<SecurityQuestionsModel>(getSecurityQuestionsRes);
        }

        /// <summary>
        /// checks account type
        /// </summary>
        /// <param name="Id"> user id </param>
        /// <returns> true or false </returns>
        public async Task<bool> GetIsIndividual(string Id)
        {
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/Account/GetIsIndividual/" + Id, true);
            return await Utility.DeserializeObject<bool>(roleResponseMessage);
        }

        /// <summary>
        /// Check user email exists or not
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> true or false </returns>
        public async Task<bool> CheckIsEmailExists(string email)
        {
            string urlAction = "api/Account/IsEmailExists/" + email + "/";
            HttpResponseMessage isEmailExistResponse = await Utility.GetObject(urlAction);
            return await Utility.DeserializeObject<bool>(isEmailExistResponse);
        }

        /// <summary>
        /// gets user id by email
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> User id </returns>
        public async Task<string> GetUserIdByEmail(string email)
        {
            string urlAction = "api/Account/GetUserIdByEmail/" + email + "/";
            HttpResponseMessage getUserIDResponse = await Utility.GetObject(urlAction);

            return await Utility.DeserializeObject<string>(getUserIDResponse);
        }

        /// <summary>
        /// checks is user family member or not
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> true or false </returns>
        public async Task<bool> IsFamilyMember(string email)
        {
            string urlAction = "api/Account/IsFamilyMember/" + email + "/";
            HttpResponseMessage isEmailExistResponse = await Utility.GetObject(urlAction);
            return await Utility.DeserializeObject<bool>(isEmailExistResponse);
        }
    }
}