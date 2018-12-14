using Chinmaya.Registration.Models;
using Chinmaya.Registration.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Chinmaya.Registration.UI.Services
{
    public class CommonService
    {
        System.Collections.Specialized.NameValueCollection configMngr = ConfigurationManager.AppSettings;

        /// <summary>
        /// gets gender data
        /// </summary>
        /// <returns> List of genders </returns>
        public async Task<List<KeyValueModel>> GetGenderData()
        {
            Utility.MasterType masterValue = Utility.MasterType.GENDER;
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/Master/GetMasterData", masterValue, true);
            return await Utility.DeserializeObject<List<KeyValueModel>>(roleResponseMessage);
        }

        /// <summary>
        /// gets age group data
        /// </summary>
        /// <returns> list of age group data</returns>
        public async Task<object> GetAgeGroupData()
        {
            Utility.MasterType masterValue = Utility.MasterType.AGEGROUPID;
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/Master/GetMasterData", masterValue, true);
            return await Utility.DeserializeList<KeyValueModel>(roleResponseMessage);
        }

        /// <summary>
        /// gets all countries
        /// </summary>
        /// <returns> list of countries </returns>
        public async Task<object> GetCountryData()
        {
            Utility.MasterType masterValue = Utility.MasterType.COUNTRY;
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/Master/GetMasterData", masterValue, true);
            return await Utility.DeserializeList<KeyValueModel>(roleResponseMessage);
        }

        /// <summary>
        /// Gets all security questions
        /// </summary>
        /// <returns> list of security questions </returns>
        public async Task<List<SecurityQuestionsModel>> GetSecurityQuestions()
        {
            Utility.MasterType masterValue = Utility.MasterType.SECURITYQUESTIONS;
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/Master/GetMasterData", masterValue, true);
            return await Utility.DeserializeList<SecurityQuestionsModel>(roleResponseMessage);
        }

        /// <summary>
        /// decrypt the encrypted content
        /// </summary>
        /// <param name="content"> encrypted content</param>
        /// <returns> decrypted content as string </returns>
        public string DecryptContent(string content)
        {
            EncryptDecrypt objEncryptDecrypt = new EncryptDecrypt();
            return objEncryptDecrypt.Decrypt(content, configMngr["ServiceAccountPassword"]);
        }

        /// <summary>
        /// encrypt the encrypted content
        /// </summary>
        /// <param name="content"> decrypted content</param>
        /// <returns> encrypted content as string </returns>
        public string EncryptContent(string content)
        {
            EncryptDecrypt objEncryptDecrypt = new EncryptDecrypt();
            return objEncryptDecrypt.Encrypt(content, configMngr["ServiceAccountPassword"]);
        }

        /// <summary>
        /// gets all relationship data
        /// </summary>
        /// <returns> list of relationship data </returns>
        public async Task<List<KeyValueModel>> GetRelationshipData()
        {
            Utility.MasterType masterValue = Utility.MasterType.RELATIONSHIP;
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/Master/GetMasterData", masterValue, true);
            return await Utility.DeserializeObject<List<KeyValueModel>>(roleResponseMessage);
        }

        /// <summary>
        /// gets all genders data
        /// </summary>
        /// <returns> list of genders </returns>
        public async Task<List<KeyValueModel>> GetGradeData()
        {
            Utility.MasterType masterValue = Utility.MasterType.GRADE;
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/Master/GetMasterData", masterValue, true);
            return await Utility.DeserializeObject<List<KeyValueModel>>(roleResponseMessage);
        }

        /// <summary>
        /// gets all weekday data
        /// </summary>
        /// <returns> list of weekdays</returns>
        public async Task<List<Weekdays>> GetWeekdayData()
        {
            Utility.MasterType masterValue = Utility.MasterType.WEEKDAY;
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/Master/GetMasterData", masterValue, true);
            return await Utility.DeserializeObject<List<Weekdays>>(roleResponseMessage);
        }

        /// <summary>
        /// gets frequency data
        /// </summary>
        /// <returns> list of frequency data </returns>
        public async Task<List<Frequencies>> GetFrequencyData()
        {
            Utility.MasterType masterValue = Utility.MasterType.FREQUENCY;
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/Master/GetMasterData", masterValue, true);
            return await Utility.DeserializeObject<List<Frequencies>>(roleResponseMessage);
        }

        /// <summary>
        /// gets session data
        /// </summary>
        /// <returns> list of sessions </returns>
        public async Task<List<Sessions>> GetSessionData()
        {
            Utility.MasterType masterValue = Utility.MasterType.SESSION;
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/Master/GetMasterData", masterValue, true);
            return await Utility.DeserializeObject<List<Sessions>>(roleResponseMessage);
        }

        /// <summary>
        /// gets list of all events
        /// </summary>
        /// <returns> list of events </returns>
        public async Task<List<CurrentEventModel>> GetEvents()
        {
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/Event/GetEventsData/", true);
            return await Utility.DeserializeObject<List<CurrentEventModel>>(roleResponseMessage);
        }

        /// <summary>
        /// gets account type
        /// </summary>
        /// <returns> account type model </returns>
        public async Task<object> GetAccountType()
        {
            Utility.MasterType masterValue = Utility.MasterType.ACCOUNTTYPE;
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/Master/GetMasterData", masterValue, true);
            return await Utility.DeserializeList<KeyValueModel>(roleResponseMessage);
        }

		/// <summary>
		/// gets role type
		/// </summary>
		/// <returns> account type model </returns>
		public async Task<string> GetUserRoleName(string email)
		{
			string urlAction = "api/User/GetUserRoleNameByEmail/" + email + "/";
			HttpResponseMessage getFullnameResponse = await Utility.GetObject(urlAction);

			return await Utility.DeserializeObject<string>(getFullnameResponse);
		}
	}
}