using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chinmaya.DAL;
using Chinmaya.Registration.DAL;
using Chinmaya.Registration.Models;
using AutoMapper;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;

namespace Chinmaya.Registration.DAL
{
    public class Events
    {
        /// <summary>
        /// gets event data by event id
        /// </summary>
        /// <param name="Id"> event id </param>
        /// <returns> current event model </returns>
        public CurrentEventModel GetEventData(string Id)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                var eventData = (from e in _ctx.Events
                                 where e.Id == Id
                                 select new CurrentEventModel
                                 {

                                     Id = e.Id,
                                     Name = e.Name,
                                     Description = e.Description,
                                     Weekday = _ctx.Weekdays.Where(i => i.Id == e.WeekdayId).Select(i => i.Name).FirstOrDefault(),
                                     Frequency = _ctx.Frequencies.Where(i => i.Id == e.FrequencyId).Select(i => i.Name).FirstOrDefault(),
                                     StartTime = _ctx.EventSessions.Where(i => i.EventId == e.Id).Select(i => i.StartTime).FirstOrDefault(),
                                     EndTime = _ctx.EventSessions.Where(i => i.EventId == e.Id).Select(i => i.EndTime).FirstOrDefault(),
                                     Amount = e.Amount
                                 }).FirstOrDefault();
                return eventData;
            }
        }

        /// <summary>
        /// gets all events data
        /// </summary>
        /// <returns> list of current event model </returns>
        public List<CurrentEventModel> GetEventsData()
        {

            using (var _ctx = new ChinmayaEntities())
            {
                var events = (from e in _ctx.Events
                              select new CurrentEventModel
                              {

                                  Id = e.Id,
                                  Name = e.Name,
                                  Description = e.Description,
                                  Weekday = _ctx.Weekdays.Where(i => i.Id == e.WeekdayId).Select(i => i.Name).FirstOrDefault(),
                                  Frequency = _ctx.Frequencies.Where(i => i.Id == e.FrequencyId).Select(i => i.Name).FirstOrDefault(),
                                  StartTime = _ctx.EventSessions.Where(i => i.EventId == e.Id).Select(i => i.StartTime).FirstOrDefault(),
                                  EndTime = _ctx.EventSessions.Where(i => i.EventId == e.Id).Select(i => i.EndTime).FirstOrDefault(),
                                  Amount = e.Amount
                              }).ToList();
                return events;
            }

        }

        /// <summary>
        /// gets all events data by user age
        /// </summary>
        /// <param name="age"> user age </param>
        /// <returns> list of current event model </returns>
        public List<CurrentEventModel> GetEventsData(int age)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                var events = (from e in _ctx.Events
                              where e.AgeFrom <= age && age <= e.AgeTo
                              select new CurrentEventModel
                              {
                                  Id = e.Id,
                                  Name = e.Name,
                                  Description = e.Description,
                                  Weekday = _ctx.Weekdays.Where(i => i.Id == e.WeekdayId).Select(i => i.Name).FirstOrDefault(),
                                  Frequency = _ctx.Frequencies.Where(i => i.Id == e.FrequencyId).Select(i => i.Name).FirstOrDefault(),
                                  StartTime = _ctx.EventSessions.Where(i => i.EventId == e.Id).Select(i => i.StartTime).FirstOrDefault(),
                                  EndTime = _ctx.EventSessions.Where(i => i.EventId == e.Id).Select(i => i.EndTime).FirstOrDefault(),
                                  AgeFrom = e.AgeFrom,
                                  AgeTo = e.AgeTo,
                                  Amount = e.Amount
                              }).ToList();
                return events;
            }

        }

        /// <summary>
        /// adds event
        /// </summary>
        /// <param name="ev"> Event Model </param>
        public string PostEvent(EventsModel ev)
        {
            using (var _ctx = new ChinmayaEntities())
            {
				string res = "";

				using (var transaction = _ctx.Database.BeginTransaction())
				{
					try
					{
						var config = new MapperConfiguration(cfg =>
						{
							cfg.CreateMap<EventsModel, Event>();
						});
						IMapper mapper = config.CreateMapper();
						Event evnt = new Event();
						mapper.Map(ev, evnt);

						var config1 = new MapperConfiguration(cfg =>
						{
							cfg.CreateMap<EventsModel, EventSession>().ForMember(dest => dest.Id, opt => opt.Ignore());
						});
						IMapper mapper1 = config1.CreateMapper();
						EventSession evntssn = new EventSession();
						mapper1.Map(ev, evntssn);

						var config2 = new MapperConfiguration(cfg =>
						{
							cfg.CreateMap<EventsModel, EventHoliday>().ForMember(dest => dest.Id, opt => opt.Ignore());
						});
						IMapper mapper2 = config2.CreateMapper();
						EventHoliday evnthld = new EventHoliday();
						mapper2.Map(ev, evnthld);

						if (ev.Id == null)
						{
							
							evnt.Id = Guid.NewGuid().ToString();
							evnt.Status = true;
							evnt.CreatedDate = DateTime.Now;
							_ctx.Events.Add(evnt);
							_ctx.SaveChanges();
							
							evntssn.EventId = evnt.Id;
							_ctx.EventSessions.Add(evntssn);
							_ctx.SaveChanges();
							
							evnthld.EventId = evnt.Id;
							_ctx.EventHolidays.Add(evnthld);
							_ctx.SaveChanges();

							transaction.Commit();
							res = "Event successfully added";
						}
						else
						{
							var rgstrusr = _ctx.EventRegistrations.Where(r => r.EventId == ev.Id).FirstOrDefault();
							if (rgstrusr == null)
							{
								evnt = _ctx.Events.Where(e => e.Id == ev.Id).FirstOrDefault();
								evnt.Id = ev.Id;
								evnt.Name = ev.Name;
								evnt.Description = ev.Description;
								evnt.WeekdayId = ev.WeekdayId;
								evnt.FrequencyId = ev.FrequencyId;
								evnt.AgeFrom = ev.AgeFrom;
								evnt.AgeTo = ev.AgeTo;
								evnt.Amount = ev.Amount;
								evnt.Status = true;
								evnt.CreatedDate = ev.CreatedDate;
								evnt.UpdatedBy = ev.UpdatedBy;
								evnt.UpdatedDate = DateTime.Now;
								_ctx.SaveChanges();

								evntssn = _ctx.EventSessions.Where(e => e.EventId == ev.Id).FirstOrDefault();
								evntssn.SessionId = ev.SessionId;
								evntssn.StartDate = ev.StartDate;
								evntssn.EndDate = ev.EndDate;
								evntssn.StartTime = ev.StartTime;
								evntssn.EndTime = ev.EndTime;
								evntssn.Location = ev.Location;
								evntssn.Instructor = ev.Instructor;
								evntssn.Contact = ev.Contact;
								evntssn.Other = ev.Other;
								_ctx.SaveChanges();

								evnthld = _ctx.EventHolidays.Where(e => e.EventId == ev.Id).FirstOrDefault();
								evnthld.HolidayDate = ev.HolidayDate;
								_ctx.SaveChanges();

								transaction.Commit();
								res = "Event successfully edited";
							}
							else
							{
								res = "Users already registered for the Event";
							}

						}
						
					}

					catch
					{
						transaction.Rollback();
						throw;
					}
				}
				return res;
			}
        }

		/// <summary>
		/// deletes Event
		/// </summary>
		/// <param name="id"> user id </param>
		/// returns string
		public string DeleteEvent(string id)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				var msg="";
				var rgstrusr = _ctx.EventRegistrations.Where(r => r.EventId == id).FirstOrDefault();
				if (rgstrusr == null)
				{
					var evnt = _ctx.Events.Where(r => r.Id == id).FirstOrDefault();
					_ctx.Events.Remove(evnt);
					msg = "Event Deleted Successfully";
				}
				else
				{
					msg = "Users already registered for the Event";
				}
				try
				{
					_ctx.SaveChanges();
				}

				catch
				{
					throw;
				}
				return msg;
			}
		}


		/// <summary>
		/// adds user to directory by using user id
		/// </summary>
		/// <param name="id"> user id </param>
		public void AddtoDirectory(string id)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                var user = _ctx.Directories.Where(r => r.UserId == id).Select(n => n.UserId).FirstOrDefault();
                if (user == null)
                {
                    var dir = new Directory
                    {
                        UserId = id
                    };
                    _ctx.Directories.Add(dir);
                }
                try
                {
                    _ctx.SaveChanges();
                }
				catch
				{
					throw;
				}
			}
        }

		/// <summary>
		/// get event details
		/// </summary>
		/// <param name="Id"> event id </param>
		/// <returns> event model </returns>
		public EventsModel GetEventDetails(string Id)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				EventsModel emm = new EventsModel();
				Event emData = _ctx.Events.Where(e => e.Id == Id).FirstOrDefault();
				if (emData != null)
				{
					emm.Id = emData.Id;
					emm.Name = emData.Name;
					emm.Description = emData.Description;
					emm.WeekdayId = emData.WeekdayId;
					emm.FrequencyId = emData.FrequencyId;
					emm.AgeFrom = emData.AgeFrom;
					emm.AgeTo = emData.AgeTo;
					emm.Amount = emData.Amount;
					EventSession esessionData = _ctx.EventSessions.Where(es => es.EventId == emData.Id).FirstOrDefault();
					emm.SessionId = esessionData.SessionId;
					emm.StartDate = Convert.ToDateTime(esessionData.StartDate);
					emm.EndDate = Convert.ToDateTime(esessionData.EndDate);
					emm.StartTime = esessionData.StartTime;
					emm.EndTime = esessionData.EndTime;
					emm.Location = esessionData.Location;
					emm.Instructor = esessionData.Instructor;
					emm.Contact = esessionData.Contact;
					emm.Other = esessionData.Other;
					EventHoliday eHolidayData = _ctx.EventHolidays.Where(eh => eh.EventId == emData.Id).FirstOrDefault();
					emm.HolidayDate = eHolidayData.HolidayDate;
				}
				return emm;
			}
		}
	}
}
