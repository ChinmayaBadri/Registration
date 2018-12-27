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
        public void PostEvent(EventsModel ev)
        {
            using (var _ctx = new ChinmayaEntities())
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
					if (string.IsNullOrEmpty(ev.Id))
					{
						evnt.Id = Guid.NewGuid().ToString();
						evnt.Status = true;
						evnt.CreatedDate = DateTime.Now;
						_ctx.Events.Add(evnt);
						_ctx.SaveChanges();
					}
					else
					{
						_ctx.Entry(evnt).State = EntityState.Modified;
						_ctx.SaveChanges();
					}

					var config1 = new MapperConfiguration(cfg =>
					{
						cfg.CreateMap<EventsModel, EventSession>();
					});
					IMapper mapper1 = config.CreateMapper();
					EventSession evntssn = new EventSession();
					mapper.Map(ev, evntssn);

					if (string.IsNullOrEmpty(ev.Id))
					{
						evntssn.EventId = evnt.Id;
						_ctx.EventSessions.Add(evntssn);
						_ctx.SaveChanges();
					}
					else
					{
						_ctx.Entry(evntssn).State = EntityState.Modified;
						_ctx.SaveChanges();
					}

					
					var config2 = new MapperConfiguration(cfg =>
					{
						cfg.CreateMap<EventsModel, EventHoliday>();
					});

					IMapper mapper2 = config.CreateMapper();
					EventHoliday evnthld = new EventHoliday();
					mapper.Map(ev, evnthld);

					if (string.IsNullOrEmpty(ev.Id))
					{
						evnthld.EventId = evnt.Id;
						_ctx.EventHolidays.Add(evnthld);
						_ctx.SaveChanges();
					}
					else
					{
						_ctx.Entry(evnthld).State = EntityState.Modified;
						_ctx.SaveChanges();
					}
					
				}

				//var eventId = "";
				//if (string.IsNullOrEmpty(ev.Id))
				//{
				//	eventId = Guid.NewGuid().ToString();
				//}
				//else
				//{
				//	eventId = ev.Id;
				//}
				//	var eve = new Event
				//	{
				//		Id = eventId,
				//		Name = ev.Name,
				//		Description = ev.Description,
				//		WeekdayId = ev.WeekdayId,
				//		FrequencyId = ev.FrequencyId,
				//		AgeFrom = ev.AgeFrom,
				//		AgeTo = ev.AgeTo,
				//		Amount = ev.Amount,
				//		Status = true,
				//		CreatedBy = ev.CreatedBy,
				//		CreatedDate = DateTime.Now
				//	};

				//	var evs = new EventSession
				//	{
				//		EventId = eve.Id,
				//		SessionId = ev.SessionId,
				//		StartDate = ev.StartDate,
				//		EndDate = ev.EndDate,
				//		StartTime = ev.StartTime,
				//		EndTime = ev.EndTime,
				//		Location = ev.Location,
				//		Instructor = ev.Instructor,
				//		Contact = ev.Contact,
				//		Other = ev.Other
				//	};


				//	if (ev.HolidayDate != null)
				//	{
				//		var eHoliday = new EventHoliday
				//		{
				//			EventId = eve.Id,
				//			HolidayDate = ev.HolidayDate
				//		};
				//		_ctx.EventHolidays.Add(eHoliday);
				//	}
				//	_ctx.Events.Add(eve);
				//	_ctx.EventSessions.Add(evs);
				
                catch (DbEntityValidationException e)
                {
                    foreach (var even in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            even.Entry.Entity.GetType().Name, even.Entry.State);
                        foreach (var ve in even.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }

                }
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
                catch (DbEntityValidationException e)
                {
                    foreach (var even in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            even.Entry.Entity.GetType().Name, even.Entry.State);
                        foreach (var ve in even.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }

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
					emm.StartDate = esessionData.StartDate;
					emm.EndDate = esessionData.EndDate;
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
