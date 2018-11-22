using AutoMapper;
using Chinmaya.DAL;
using Chinmaya.Models;
using Chinmaya.Registration.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinmaya.Registration.DAL
{
	[DbConfigurationType(typeof(CodeConfig))] // point to the class that inherit from DbConfiguration
	public class Users
	{
		//Get Specific user details based on username and password
		public UserModel GetUserInfo(LoginViewModel entity)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				User objUserInfo = _ctx.Users.Where(u => u.Email == entity.UserName && u.Password == entity.Password).FirstOrDefault();

				var config = new MapperConfiguration(cfg =>
				{
					cfg.CreateMap<User, UserModel>();
				});
				IMapper mapper = config.CreateMapper();

				UserModel ur = new UserModel();
				return mapper.Map(objUserInfo, ur);

				//return Mapper.Map<User, UserModel>(objUserInfo);
			}
		}
		public string GetRoleName(int id)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				return _ctx.Roles.Where(r => r.Id == id).Select(n => n.Name).FirstOrDefault();
			}
		}

		public List<string> GetUsers()
		{
			return new List<string>
			{
				"User1",
				"User2",
				"User3"
			};
		}

		public List<GetFamilyMemberForUser_Result> GetUserFamilyMemberData(string Id)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				return _ctx.GetFamilyMemberForUser(Id).ToList();
			}
		}

		//public FamilyMemberModel GetFamilyMemberDetails(string Id)
		//{
		//	using (var _ctx = new ChinmayaEntities())
		//	{
		//		var userData = (from e in _ctx.Users
		//						where e.Id == Id
		//						select new FamilyMemberModel
		//						{
		//							FirstName = e.FirstName,
		//							LastName = e.LastName,
		//							DOB = e.DOB,
		//							//RelationshipData = e.RelationshipId,
		//							//Grade = (int)e.GradeId,
		//							GenderData = e.GenderId,
		//							CellPhone = e.CellPhone,
		//							Email = e.Email
		//						}).FirstOrDefault();
		//		if (userData == null)
		//		{
		//			userData = (from e in _ctx.FamilyMembers
		//						where e.Id == Id
		//						select new FamilyMemberModel
		//						{
		//							FirstName = e.FirstName,
		//							LastName = e.LastName,
		//							DOB = e.DOB,
		//							RelationshipData = e.RelationshipId,
		//							Grade = (int)e.GradeId,
		//							GenderData = e.GenderId,
		//							CellPhone = e.CellPhone,
		//							Email = e.Email
		//						}).FirstOrDefault();
		//		}
		//			return userData;
		//	}
		//}

		public bool GetIsIndividual(string Id)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				return _ctx.Users.Where(r => r.Id == Id).Select(n => n.IsIndividual).FirstOrDefault();
			}
		}

		public UserFamilyMember GetUserData(string Id)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				var userData = (from e in _ctx.Users
								where e.Id == Id
								select new UserFamilyMember
								{

									Id = e.Id,
									FirstName = e.FirstName,
									LastName = e.LastName,
								}).FirstOrDefault();

				if (userData == null)
				{
					userData = (from e in _ctx.FamilyMembers
									where e.Id == Id
									select new UserFamilyMember
									{

										Id = e.Id,
										FirstName = e.FirstName,
										LastName = e.LastName,
									}).FirstOrDefault();
				}
				return userData;
			}
		}

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

								  Weekday = _ctx.Weekdays.Where(i => i.Id == e.WeekdayId).Select(i => i.Name).FirstOrDefault(),
								  Frequency = _ctx.Frequencies.Where(i => i.Id == e.FrequencyId).Select(i => i.Name).FirstOrDefault(),
								  StartTime = _ctx.EventSessions.Where(i => i.EventId == e.Id).Select(i => i.StartTime).FirstOrDefault(),
								  EndTime = _ctx.EventSessions.Where(i => i.EventId == e.Id).Select(i => i.EndTime).FirstOrDefault(),
								  Amount = e.Amount
							  }).ToList();
				return events;
			}
			
		}

		
		//public List<ProgramEventRegistrationModel> GetEventsListData(string Id)
		//{
		//	//var config = new MapperConfiguration(cfg =>
		//	//{
		//	//	cfg.CreateMap<EventsModel, Event>().ReverseMap();
		//	//});
		//	//IMapper mapper = config.CreateMapper();


		//	using (var _ctx = new ChinmayaEntities())
		//	{
		//		//var _eveData = mapper.Map<List<EventsModel>>(_ctx.Events);

		//		//foreach (var item in _eveData)
		//		//{
		//		//	var EveSession = _ctx.EventSessions.FirstOrDefault(x => x.EventId == item.Id);
		//		//	var Eveweek = _ctx.Weekdays.FirstOrDefault(x => x.Id == item.WeekdayId);
		//		//	item.StartTime = EveSession.StartTime;
		//		//	item.EndTime = EveSession.EndTime;
		//		//	item.WeekdayName = Eveweek.Name;
		//		//}

		//		var events = (from f in _ctx.FamilyMembers
		//					  where f.UpdatedBy == Id
		//					  select new ProgramEventRegistrationModel
		//					  {

		//						  UserId = f.Id,
		//						  FirstName = f.FirstName,
		//						  LastName = f.LastName,
		//						  Events = (from e in _ctx.Events
		//									select new EventsModel
		//									{
		//										Id = e.Id,
		//										Name = e.Name,
		//										WeekdayName = _ctx.Weekdays.Where(i => i.Id == e.WeekdayId).Select(i => i.Name).FirstOrDefault(),
		//										StartTime = _ctx.EventSessions.Where(i => i.EventId == e.Id).Select(i => i.StartTime).FirstOrDefault(),
		//										EndTime = _ctx.EventSessions.Where(i => i.EventId == e.Id).Select(i => i.EndTime).FirstOrDefault(),
		//										Amount = e.Amount
		//									})
		//					  }).ToList();
		//		return events;
		//	}

		//}

		public void PostUser(UserModel user)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				var config = new MapperConfiguration(cfg =>
				{
					cfg.CreateMap<UserModel, User>();
				});
				IMapper mapper = config.CreateMapper();

				User ur = new User();
				mapper.Map(user, ur);
				ur.Id = Guid.NewGuid().ToString();
				ur.CreatedDate = DateTime.Now;
				ur.RoleId = 2;
				Dictionary<int, string> SecurityQuestions = new Dictionary<int, string>();
				SecurityQuestions = user.UserSecurityQuestions;
				UserSecurityQuestion usq = new UserSecurityQuestion();
				foreach (var item in SecurityQuestions)
				{
					var UserSQ = new UserSecurityQuestion();
					UserSQ.UserId = ur.Id;
					UserSQ.SecurityQuestionId = item.Key;
					UserSQ.Answer = item.Value;
					_ctx.UserSecurityQuestions.Add(UserSQ);
				}
				
				
				_ctx.Users.Add(ur);

				//UserSecurityQuestion Usq = new UserSecurityQuestion();

				//for (int i = 1; i <= Count; i++)
				//{
				//	Usq.UserId = ur.Id;
				//	Usq.SecurityQuestionId = x.Id;
				//	Usq.Answer = x.Value;
				//}
				//_ctx.UserSecurityQuestions.Add(Usq);



				//var gff = Mapper.Map<UserModel, User>(user);
				//var ur = new User
				//{
				//	Id = Guid.NewGuid().ToString(),
				//	Email = user.Email,
				//	EmailConfirmed = false,
				//	Password = user.Password,
				//	RoleId = 1,
				//	FirstName = user.FirstName,
				//	LastName = user.LastName,
				//	DOB = user.DOB,
				//	AgeGroupId = null,
				//	GenderId = user.GenderId,
				//	Address = user.Address,
				//	CityId = user.CityId,
				//	StateId = user.StateId,
				//	CountryId = user.CountryId,
				//	ZipCode = user.ZipCode,
				//	HomePhone = user.HomePhone,
				//	IsIndividual = true,
				//	Status = true
				//};
				//_ctx.Users.Add(gff);

				//_ctx.Users.Add(user);
				//_ctx.SaveChanges();
				try
				{
					_ctx.SaveChanges();
				}
				catch (DbEntityValidationException e)
				{
					foreach (var eve in e.EntityValidationErrors)
					{
						Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
							eve.Entry.Entity.GetType().Name, eve.Entry.State);
						foreach (var ve in eve.ValidationErrors)
						{
							Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
								ve.PropertyName, ve.ErrorMessage);
						}
					}

				}
			}
		}


		public void PostFamilyMember(FamilyMemberModel family)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				var config = new MapperConfiguration(cfg =>
				{
					cfg.CreateMap<FamilyMemberModel, FamilyMember>();
				});
				IMapper mapper = config.CreateMapper();

				//FamilyMember fm = new FamilyMember();
				//mapper.Map(family, fm);
				//fm.Id = Guid.NewGuid().ToString();
				//fm.Status = true;

				var email = _ctx.Users.Where(r => r.Email == family.Email).Select(n => n.Email).FirstOrDefault();
				var HomePhone = _ctx.Users.Where(r => r.HomePhone == family.CellPhone).Select(n => n.CellPhone).FirstOrDefault();
				//if (email != null || HomePhone != null)
				//{
					
				//}
				var fm = new FamilyMember
				{
					Id = Guid.NewGuid().ToString(),
					FirstName = family.FirstName,
					LastName = family.LastName,
					DOB = family.DOB,
					RelationshipId = family.RelationshipData,
					GradeId = family.Grade,
					GenderId = family.GenderData,
					CellPhone = family.CellPhone,
					Email = family.Email,
					Status = true,
					UpdatedBy = family.UpdatedBy,
					UpdatedDate = DateTime.Now
				};

				_ctx.FamilyMembers.Add(fm);
								
				try
				{
					_ctx.SaveChanges();
				}
				catch (DbEntityValidationException e)
				{
					foreach (var eve in e.EntityValidationErrors)
					{
						Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
							eve.Entry.Entity.GetType().Name, eve.Entry.State);
						foreach (var ve in eve.ValidationErrors)
						{
							Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
								ve.PropertyName, ve.ErrorMessage);
						}
					}

				}
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

				//Event eve = new Event();
				//mapper.Map(ev, eve);
				//eve.Id = Guid.NewGuid().ToString();
				//eve.Status = true;


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

		public void PostCheckPayment(CheckPaymentModel chkp)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				var config = new MapperConfiguration(cfg =>
				{
					cfg.CreateMap<CheckPaymentModel, CheckPayment>();
				});
				IMapper mapper = config.CreateMapper();

				//CheckPayment chk = new CheckPayment();
				//mapper.Map(chkp, chk);
				//chk.CreatedDate = DateTime.Now;
				//chk.StatusId = 1;


				var chk = new CheckPayment
				{
					AccountHolderName = chkp.AccountHolderName,
					AccountTypeId = chkp.AccountTypeId,
					BankName = chkp.BankName,
					AccountNumber = chkp.AccountNumber,
					RoutingNumber = chkp.RoutingNumber,
					Amount = chkp.Amount,
					StatusId = 1,
					CreatedBy = chkp.CreatedBy,
					CreatedDate = DateTime.Now
				};

				_ctx.CheckPayments.Add(chk);
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

