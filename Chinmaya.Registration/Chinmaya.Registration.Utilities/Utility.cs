using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json.Converters;
using System.Web.Configuration;
using System.Net.Mail;
using System.Net;
using log4net;

namespace Chinmaya.Registration.Utilities
{
    public sealed class Utility
    {
        static string baseURL = WebConfigurationManager.AppSettings["BaseURL"];
        [JsonConverter(typeof(StringEnumConverter))]
        public enum MasterType
        {
            ROLE = 0, ACCOUNT = 1, GENDER = 2, COUNTRY = 3, STATE = 4, CITY = 5, SECURITYQUESTIONS = 6, AGEGROUPID = 7, RELATIONSHIP = 8, GRADE = 9, WEEKDAY = 10, FREQUENCY = 11, ACCOUNTTYPE = 12, SESSION = 13
        }

        /// <summary>
        /// This method handles the requests and response of http request and returns HttpResponse Message
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="uriActionString"> Action string </param>
        /// <param name="content"> incoming model content </param>
        /// <param name="auth"> authorization </param>
        /// <returns> HttpResponse Message </returns>
        public static async Task<HttpResponseMessage> GetObject<T1>(string uriActionString, T1 content, bool auth = true)
        {
            HttpResponseMessage res = new HttpResponseMessage();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseURL);
                    client.DefaultRequestHeaders.Accept.Clear();

                    HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, uriActionString);
                    req.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
                    res = await client.SendAsync(req);
                }
                return res;
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        /// <summary>
        /// gets http response message by sending request
        /// </summary>
        /// <param name="uriActionString">uri Action String</param>
        /// <param name="auth"> authorization </param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> GetObject(string uriActionString, bool auth = true)
        {
            HttpResponseMessage res = new HttpResponseMessage();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseURL);
                    client.DefaultRequestHeaders.Accept.Clear();

                    HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uriActionString);
                    res = await client.SendAsync(req);
                }
                return res;
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        /// <summary>
        /// This method deserialize the incoming Http response into type paramater that you've passed while
        /// calling this method.
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="res"></param>
        /// <returns></returns>
        public static async Task<T> DeserializeObject<T>(HttpResponseMessage res)
        {
            return await Task.Run(() => GetAnObject<T>(res));
        }

        /// <summary>
        /// gets deserialized object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="res"></param>
        /// <returns> deserialized object</returns>
        public static T GetAnObject<T>(HttpResponseMessage res)
        {
            T returnValue = default(T);
            returnValue = JsonConvert.DeserializeObject<T>(res.Content.ReadAsStringAsync().Result);
            return returnValue;
        }

        /// <summary>
        /// This method deserialize the incoming Http response into type paramater that you've passed while
        /// calling this method.
        /// </summary>
        /// <typeparam name="T"> return  model type </typeparam>
        /// <param name="res"></param>
        /// <returns> type model </returns>
        public static async Task<List<T>> DeserializeList<T>(HttpResponseMessage res)
        {
            return await Task.Run(() => GetList<T>(res));
        }

        /// <summary>
        /// gets deserialized list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="res"></param>
        /// <returns> deserialized list</returns>
        public static List<T> GetList<T>(HttpResponseMessage res)
        {
            List<T> returnValue = default(List<T>);
            returnValue = JsonConvert.DeserializeObject<List<T>>(((HttpResponseMessage)res).Content.ReadAsStringAsync().Result);
            return returnValue;
        }

        public static ILog getLogger(Type moduleType)
        {
            ILog logger = LogManager.GetLogger(moduleType);
            return logger;
        }
}
}
