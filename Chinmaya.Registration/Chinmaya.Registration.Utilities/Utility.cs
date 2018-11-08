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

namespace Chinmaya.Registration.Utilities
{
    public sealed class Utility
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum MasterType
        {
            ROLE = 0, ACCOUNT = 1, GENDER = 2, COUNTRY = 3, STATE = 4, CITY =5, SECURITYQUESTIONS =6, AGEGROUPID =7, RELATIONSHIP =8, GRADE =9, WEEKDAY =10, FREQUENCY =11, ACCOUNTTYPE =12, SESSION =13
        }

        public static async Task<String> PostAnObject<T, T1>(string uriActionString, T1 content, bool auth = true)
        {
            String returnValue = String.Empty;
            var client = new HttpClient();
            
            client.BaseAddress = new Uri(@"http://localhost:58742/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, uriActionString);
            req.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            var res = await client.SendAsync(req);
            if(res.IsSuccessStatusCode) return res.Content.ReadAsStringAsync().Result;

            return returnValue;
        }

        public static async Task<String> GetAnObject<T>(string uriActionString, bool auth = true)
        {
            String returnValue = String.Empty;
            var client = new HttpClient();

            client.BaseAddress = new Uri(@"http://localhost:58742/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uriActionString);
            
            var res = await client.SendAsync(req);
            if (res.IsSuccessStatusCode) return res.Content.ReadAsStringAsync().Result;

            return returnValue;
        }

        /// <summary>
        /// This method handles the requests and response of http request and returns HttpResponse Message
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="uriActionString"></param>
        /// <param name="content"></param>
        /// <param name="auth"></param>
        /// <returns> HttpResponse Message </returns>
        public static async Task<HttpResponseMessage> GetObject<T1>(string baseURL, string uriActionString, T1 content, bool auth = true)
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

        public static async Task<HttpResponseMessage> GetObject(string baseURL, string uriActionString, bool auth = true)
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

        public static async Task<Tuple<T, HttpResponseMessage>> GetList<T, T1>(T1 req, string uriActionString, string baseURL)
        {
            T responnseType = default(T);
            HttpResponseMessage res = await GetObject(baseURL, uriActionString, req, true);
            if (res.IsSuccessStatusCode)
            {
                responnseType = await DeserializeObject<T>(res);
            }

            return new Tuple<T, HttpResponseMessage>(responnseType, res);
        }
        /// <summary>
        /// This method deserialize the incoming Http response into type paramater that you've passed while
        /// calling this method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="res"></param>
        /// <returns></returns>
        public static async Task<T> DeserializeObject<T>(HttpResponseMessage res)
        {
            return await Task.Run(() => GetAnObject<T>(res));
        }

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
        /// <typeparam name="T"></typeparam>
        /// <param name="res"></param>
        /// <returns></returns>
        public static async Task<List<T>> DeserializeList<T>(HttpResponseMessage res)
        {
            return await Task.Run(() => GetList<T>(res));
        }

        public static List<T> GetList<T>(HttpResponseMessage res)
        {
            List<T> returnValue = default(List<T>);
            returnValue = JsonConvert.DeserializeObject<List<T>>(((HttpResponseMessage)res).Content.ReadAsStringAsync().Result);
            return returnValue;
        }
    }
}