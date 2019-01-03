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

        /// <summary>
        /// gets event data by event id
        /// </summary>
        /// <param name="Id"> event id </param>
        /// <returns> current event model </returns>
        [Route("api/Event/GetEventData/{id}")]
        [HttpGet]
        public CurrentEventModel GetEventData(string id)
        {
            return _event.GetEventData(id);
        }

        /// <summary>
        /// gets all events data
        /// </summary>
        /// <returns> list of current event model </returns>
        [Route("api/Event/GetEventsData/")]
        [HttpGet]
        public IEnumerable<CurrentEventModel> GetEventsData()
        {
            return _event.GetEventsData();
        }

        /// <summary>
        /// gets all events data by user age
        /// </summary>
        /// <param name="age"> user age </param>
        /// <returns> list of current event model </returns>
        [Route("api/Event/GetEventsData/{age}")]
        [HttpGet]
        public IEnumerable<CurrentEventModel> GetEventsData(int age)
        {
            return _event.GetEventsData(age);
        }

        /// <summary>
        /// adds event
        /// </summary>
        /// <param name="ev"> Event Model </param>
        [Route("api/Event/PostEvent")]
        [HttpPost]
        public string PostEvent(EventsModel obj)
        {
            try
            {
                string res = _event.PostEvent(obj);
                return res;
            }
            catch (Exception)
            {
                return "Something went wrong";
            }
        }

		/// <summary>
		/// adds event
		/// </summary>
		/// <param name="ev"> Event Model </param>
		[Route("api/Event/DeleteEvent/{id}")]
		[HttpPost]
		public string DeleteEvent(string Id)
		{
			try
			{
				return _event.DeleteEvent(Id);
			}
			catch (Exception)
			{
				return "Something went wrong";
			}
		}

		/// <summary>
		/// adds user to directory by using user id
		/// </summary>
		/// <param name="id"> user id </param>
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

		/// <summary>
		/// get event details
		/// </summary>
		/// <param name="Id"> event id </param>
		/// <returns> event model </returns>
		[Route("api/Event/GetEventDetails/{id}")]
		[HttpGet]
		public EventsModel GetEventDetails(string id)
		{
			return _event.GetEventDetails(id);
		}
	}
}
