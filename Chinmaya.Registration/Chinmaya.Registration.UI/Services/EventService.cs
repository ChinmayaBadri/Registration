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
        public async Task<CurrentEventModel> GetEventData(string Id)
        {
            HttpResponseMessage roleResponseMessage = await Utility.GetObject("/api/Event/GetEventData/" + Id, true);
            return await Utility.DeserializeObject<CurrentEventModel>(roleResponseMessage);
        }
    }
}