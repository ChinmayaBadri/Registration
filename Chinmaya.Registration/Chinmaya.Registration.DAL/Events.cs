using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chinmaya.DAL;
using Chinmaya.Registration.DAL;
using Chinmaya.Registration.Models;
using AutoMapper;
using System.Data.Entity.Validation;

namespace Chinmaya.Registration.DAL
{
    public class Events
    {
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

        public void PostEvent(EventsModel ev)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<EventsModel, Event>();
                });
                IMapper mapper = config.CreateMapper();

                var eve = new Event
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = ev.Name,
                    Description = ev.Description,
                    WeekdayId = ev.WeekdayId,
                    FrequencyId = ev.FrequencyId,
                    AgeFrom = ev.AgeFrom,
                    AgeTo = ev.AgeTo,
                    Amount = ev.Amount,
                    Status = true,
                    CreatedBy = ev.CreatedBy,
                    CreatedDate = DateTime.Now
                };

                var evs = new EventSession
                {
                    EventId = eve.Id,
                    SessionId = ev.SessionId,
                    StartDate = ev.StartDate,
                    EndDate = ev.EndDate,
                    StartTime = ev.StartTime,
                    EndTime = ev.EndTime,
                    Location = ev.Location,
                    Instructor = ev.Instructor,
                    Contact = ev.Contact,
                    Other = ev.Other
                };


                if (ev.HolidayDate != null)
                {
                    var eHoliday = new EventHoliday
                    {
                        EventId = eve.Id,
                        HolidayDate = ev.HolidayDate
                    };
                    _ctx.EventHolidays.Add(eHoliday);
                }
                _ctx.Events.Add(eve);
                _ctx.EventSessions.Add(evs);

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
    }
}
