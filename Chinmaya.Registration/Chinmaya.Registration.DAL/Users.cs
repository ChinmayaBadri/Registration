using AutoMapper;
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
				return Mapper.Map<User, UserModel>(objUserInfo);
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
	}
}

