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
        public async Task<List<KeyValueModel>> GetGenderData()
        {
            Utility.MasterType masterValue = Utility.MasterType.GENDER;
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/MasterAPI/GetMasterData", masterValue, true);
            return await Utility.DeserializeObject<List<KeyValueModel>>(roleResponseMessage);
        }

        public async Task<object> GetAgeGroupData()
        {
            Utility.MasterType masterValue = Utility.MasterType.AGEGROUPID;
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/MasterAPI/GetMasterData", masterValue, true);
            return await Utility.DeserializeList<KeyValueModel>(roleResponseMessage);
        }

        public async Task<object> GetCountryData()
        {
            Utility.MasterType masterValue = Utility.MasterType.COUNTRY;
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/MasterAPI/GetMasterData", masterValue, true);
            return await Utility.DeserializeList<KeyValueModel>(roleResponseMessage);
        }

        public async Task<List<SecurityQuestionsModel>> GetSecurityQuestions()
        {
            Utility.MasterType masterValue = Utility.MasterType.SECURITYQUESTIONS;
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/MasterAPI/GetMasterData", masterValue, true);
            return await Utility.DeserializeList<SecurityQuestionsModel>(roleResponseMessage);
        }

        public string DecryptContent(string content)
        {
            EncryptDecrypt objEncryptDecrypt = new EncryptDecrypt();
            return objEncryptDecrypt.Decrypt(content, configMngr["ServiceAccountPassword"]);
        }

        public string EncryptContent(string content)
        {
            EncryptDecrypt objEncryptDecrypt = new EncryptDecrypt();
            return objEncryptDecrypt.Encrypt(content, configMngr["ServiceAccountPassword"]);
        }

        public async Task<List<KeyValueModel>> GetRelationshipData()
        {
            Utility.MasterType masterValue = Utility.MasterType.RELATIONSHIP;
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/MasterAPI/GetMasterData", masterValue, true);
            return await Utility.DeserializeObject<List<KeyValueModel>>(roleResponseMessage);
        }

        public async Task<List<KeyValueModel>> GetGradeData()
        {
            Utility.MasterType masterValue = Utility.MasterType.GRADE;
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/MasterAPI/GetMasterData", masterValue, true);
            return await Utility.DeserializeObject<List<KeyValueModel>>(roleResponseMessage);
        }

        public async Task<List<Weekdays>> GetWeekdayData()
        {
            Utility.MasterType masterValue = Utility.MasterType.WEEKDAY;
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/MasterAPI/GetMasterData", masterValue, true);
            return await Utility.DeserializeObject<List<Weekdays>>(roleResponseMessage);
        }

        public async Task<List<Frequencies>> GetFrequencyData()
        {
            Utility.MasterType masterValue = Utility.MasterType.FREQUENCY;
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/MasterAPI/GetMasterData", masterValue, true);
            return await Utility.DeserializeObject<List<Frequencies>>(roleResponseMessage);
        }

        public async Task<List<Sessions>> GetSessionData()
        {
            Utility.MasterType masterValue = Utility.MasterType.SESSION;
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/MasterAPI/GetMasterData", masterValue, true);
            return await Utility.DeserializeObject<List<Sessions>>(roleResponseMessage);
        }

        public async Task<List<CurrentEventModel>> GetEvents()
        {
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/UserAPI/GetEventsData/", true);
            return await Utility.DeserializeObject<List<CurrentEventModel>>(roleResponseMessage);
        }
    }
}