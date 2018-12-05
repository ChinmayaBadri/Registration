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
    public class EventService
    {
        /// <summary>
        /// gets event data by event id
        /// </summary>
        /// <param name="Id"> event id</param>
        /// <returns> list of events </returns>
        public async Task<CurrentEventModel> GetEventData(string Id)
        {
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/Event/GetEventData/" + Id, true);
            return await Utility.DeserializeObject<CurrentEventModel>(roleResponseMessage);
        }
    }
}