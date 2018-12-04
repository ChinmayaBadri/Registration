using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Chinmaya.Registration.DAL;
using Chinmaya.Registration.Models;

namespace Chinmaya.BAL.Controllers
{
    public class EventController : ApiController
    {
        Events _event = new Events();

        [Route("api/Event/GetEventData/{id}")]
        [HttpGet]
        public CurrentEventModel GetEventData(string id)
        {
            return _event.GetEventData(id);
        }

        [Route("api/Event/GetEventsData/")]
        [HttpGet]
        public IEnumerable<CurrentEventModel> GetEventsData()
        {
            return _event.GetEventsData();
        }

        [Route("api/Event/GetEventsData/{age}")]
        [HttpGet]
        public IEnumerable<CurrentEventModel> GetEventsData(int age)
        {
            return _event.GetEventsData(age);
        }


        [Route("api/Event/PostEvent")]
        [HttpPost]
        public IHttpActionResult PostEvent(EventsModel obj)
        {
            try
            {
                _event.PostEvent(obj);
                return Ok("Success");
            }
            catch (Exception)
            {
                return Ok("Something went wrong");
            }
        }

        [Route("api/Event/AddtoDirectory/{id}")]
        [HttpPost]
        public IHttpActionResult AddtoDirectory(string id)
        {
            try
            {
                _event.AddtoDirectory(id);
                return Ok("Success");
            }
            catch (Exception)
            {
                return Ok("Something went wrong");
            }
        }
    }
}
