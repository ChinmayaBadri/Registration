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
        /// <summary>
        /// gets user info by email
        /// </summary>
        /// <param name="entity"> Login View Model </param>
        /// <returns> User model </returns>
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
			}
		}

        /// <summary>
        /// gets user role name
        /// </summary>
        /// <param name="id"> user id </param>
        /// <returns> user role </returns>
		public string GetRoleName(int id)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				return _ctx.Roles.Where(r => r.Id == id).Select(n => n.Name).FirstOrDefault();
			}
		}

        /// <summary>
        /// gets user family members data by user id
        /// </summary>
        /// <param name="Id"> user id </param>
        /// <returns> User Family member model </returns>
		public List<GetFamilyMemberForUser_Result> GetUserFamilyMemberData(string Id)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				return _ctx.GetFamilyMemberForUser(Id).ToList();
			}
		}

        /// <summary>
        /// gets all users details
        /// </summary>
        /// <returns> list of users object </returns>
		public object GetAllUsers()
		{
			using (var _ctx = new ChinmayaEntities())
			{
				var users = (from u in _ctx.Users
						 where !_ctx.FamilyMembers.Any(f => f.Email == u.Email)
						 select new
						 {
							 Id = u.Id,
							 FullName = u.FirstName + " " + u.LastName,
							 AccountType = (u.IsIndividual) ? "Individual Account" : "Family Account",
							 DOB = u.DOB,
							 HomePhone = u.HomePhone,
							 CellPhone = u.CellPhone
						 }).ToList().Select(x => new
						 {
							 x.Id,
							 x.FullName,
							 x.AccountType,
							 DOB = string.Format("{0:MM/dd/yyyy}", x.DOB),
							 x.HomePhone,
							 x.CellPhone
						 });

				return users;
			}
		}

        /// <summary>
        /// gets all family members details
        /// </summary>
        /// <param name="id"> user id </param>
        /// <returns> list of family members object </returns>
		public object GetAllFamilyMembers(string id)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				var familyMembers = (from f in _ctx.FamilyMembers
							   where f.UpdatedBy == id
							   select new
							   {
								 Id = f.Id,
								 FullName = f.FirstName + " " + f.LastName,
								 DOB = f.DOB,
								Relationship = _ctx.Relationships.Where(i => i.Id == f.RelationshipId).Select(i => i.Name).FirstOrDefault(),
								Grade = _ctx.Grades.Where(i => i.Id == f.GradeId).Select(i => i.Name).FirstOrDefault()
							   }).ToList().Select(x => new
							   {
								   x.Id,
								   x.FullName,
								   DOB = string.Format("{0:MM/dd/yyyy}", x.DOB),
								   Relationship = (x.Relationship != null) ? x.Relationship: "-",
								   Grade = (x.Grade != null) ? x.Grade : "-"
							   });
				return familyMembers;
			}
		}

        /// <summary>
        /// gets user data
        /// </summary>
        /// <param name="Id"> user id </param>
        /// <returns>User Family Member model </returns>
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

        /// <summary>
        /// add or update user data
        /// </summary>
        /// <param name="user"> User Model </param>
		public void PostUser(UserModel user)
		{
			using (var _ctx = new ChinmayaEntities())
			{               
                try
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<UserModel, User>();
                    });
                    IMapper mapper = config.CreateMapper();

                    User ur = new User();
                    mapper.Map(user, ur);

                    if (string.IsNullOrEmpty(user.Id))
                    {
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
                            _ctx.Users.Add(ur);
                        }
                        _ctx.SaveChanges();
                    }
                    else
                    {
                        _ctx.Entry(ur).State = EntityState.Modified;
                        _ctx.SaveChanges();
                    }
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

        /// <summary>
        /// gets User phone no.
        /// </summary>
        /// <param name="Email"> user email </param>
        /// <returns> Update Phone model </returns>
		public UpdatePhone getPhoneNumber(string Id)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				UpdatePhone phone = new UpdatePhone();
				var pData = _ctx.Users.Where(f => f.Email == Id).FirstOrDefault();
				if (pData != null)
				{
					phone.Email = Id;
					phone.OldPhone = pData.CellPhone;
				}
				return phone;
			}
		}

        /// <summary>
        /// gets user email
        /// </summary>
        /// <param name="Email"> user email </param>
        /// <returns> Update email model  </returns>
		public UpdateEmail getEmail(string Id)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				UpdateEmail mail = new UpdateEmail();
				var eData = _ctx.Users.Where(f => f.Email == Id).FirstOrDefault();
				if (eData != null)
				{
					mail.email = Id;
				}
				return mail;
			}
		}

        /// <summary>
        /// gets user address
        /// </summary>
        /// <param name="Email"> user email </param>
        /// <returns> Contact Details model </returns>
		public ContactDetails getAddress(string Id)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				ContactDetails cd = new ContactDetails();
				var aData = _ctx.Users.Where(f => f.Email == Id).FirstOrDefault();
				if (aData != null)
				{
					cd.Address = aData.Address;
					cd.City = aData.City;
					cd.State = aData.StateId;
					cd.Country = aData.CountryId;
					cd.ZipCode = aData.ZipCode;
					cd.HomePhone = aData.HomePhone;
				}
				return cd;
			}
		}

        /// <summary>
        /// get family member details
        /// </summary>
        /// <param name="Id"> family member id </param>
        /// <returns> family member model </returns>
		public FamilyMemberModel GetFamilyMemberDetails(string Id)
		{
			using (var _ctx = new ChinmayaEntities())
			{
                FamilyMemberModel fmm = new FamilyMemberModel();
				FamilyMember fmData = _ctx.FamilyMembers.Where(f => f.Id == Id).FirstOrDefault();
                if(fmData != null)
                {
                    fmm = Mapper.Map(fmData, fmm);
                    fmm.GenderData = fmData.GenderId;
                    fmm.Grade = (int)fmData.GradeId;
                    fmm.RelationshipData = fmData.RelationshipId;
                }
				return fmm;
			}
		}

        /// <summary>
        /// add or edit family member details
        /// </summary>
        /// <param name="family"> Family Member Model </param>
		public void PostFamilyMember(FamilyMemberModel family)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				try
				{
					var fm = new FamilyMember();
                    fm.FirstName = family.FirstName;
                    fm.LastName = family.LastName;
                    fm.DOB = (DateTime)family.DOB;
                    fm.RelationshipId = family.RelationshipData;
                    fm.GradeId = family.Grade;
                    fm.GenderId = family.GenderData;
                    fm.CellPhone = family.CellPhone;
                    fm.Email = family.Email;
                    fm.Status = true;
                    fm.UpdatedBy = family.UpdatedBy;
                    fm.UpdatedDate = DateTime.Now;

                    if (string.IsNullOrEmpty(family.Id))
					{
						fm.Id = Guid.NewGuid().ToString();
                        _ctx.FamilyMembers.Add(fm);
                        _ctx.SaveChanges();
					}

					else
					{
                        fm.Id = family.Id;
                        _ctx.Entry(fm).State = EntityState.Modified;
                        _ctx.SaveChanges();
					}
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

		/// <summary>
		/// updates password
		/// </summary>
		/// <param name="pwd"> Update Password Model </param>
		/// <returns> true or false </returns>
		public bool UpdatePassword(UpdatePasswordModel pwd)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				var userEmail = _ctx.Users.Where(r => r.Email == pwd.Email && r.Password == pwd.OldPassword).FirstOrDefault();

				if (userEmail == null)
				{
					return false;
				}

				else if (userEmail != null)
				{
					userEmail.Password = pwd.NewPassword;
				}

				try
				{
					_ctx.Entry(userEmail).State = EntityState.Modified;
					_ctx.SaveChanges();
					return true;
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
					return false;
				}
				
			}
		}

        /// <summary>
        /// updates phone number
        /// </summary>
        /// <param name="phone"> phone number </param>
		public void UpdatePhone(UpdatePhone phone)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				var user = _ctx.Users.Where(r => r.Email == phone.Email).FirstOrDefault();
				if (user != null)
				{
					user.CellPhone = phone.OldPhone;
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
        /// updates email address
        /// </summary>
        /// <param name="el"> update email model </param>
        /// <returns> true or false </returns>
		public bool UpdateEmailAddress(UpdateEmail el)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				var user = _ctx.Users.Where(r => r.Id == el.userId).FirstOrDefault();
				if (user != null)
				{
					user.Email = el.email;
				}
				try
				{
					_ctx.Entry(user).State = EntityState.Modified;
					_ctx.SaveChanges();
					return true;
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
					return false;

				}
			}
		}

        /// <summary>
        /// updates user address
        /// </summary>
        /// <param name="cd"> Contact Details model </param>
		public void UpdateAddress(ContactDetails cd)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				var user = _ctx.Users.Where(r => r.Email == cd.Email).FirstOrDefault();
				if (user != null)
				{
					user.Address = cd.Address;
					user.City = cd.City;
					user.StateId = cd.State;
					user.CountryId = cd.Country;
					user.ZipCode = cd.ZipCode;
					user.HomePhone = cd.HomePhone;
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
        /// get user info by email address
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> User Model </returns>
        public UserModel GetUserInfoByEmail(string email)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                UserModel um = new UserModel();
                var objUser = _ctx.Users.FirstOrDefault(x => x.Email == email);
                if (objUser != null)
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<User, UserModel>();
                    });
                    IMapper mapper = config.CreateMapper();
                    return mapper.Map(objUser, um);
                }
                return um;
            }
        }

        /// <summary>
        /// gets user full name by email
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> user full name </returns>
        public string GetUserFullNameByEmail(string email)
        {
            string fullname = string.Empty;
            if (!string.IsNullOrEmpty(email))
            {
                using (var _ctx = new ChinmayaEntities())
                {
                    var objUser = _ctx.Users.FirstOrDefault(x => x.Email == email);
                    if (objUser != null)
                    {
                        fullname = objUser.FirstName + " " + objUser.LastName;
                    }
                    else
                    {
                        var objFamilyUser = _ctx.FamilyMembers.FirstOrDefault(x => x.Email == email);
                        if (objFamilyUser != null)
                        {
                            fullname = objFamilyUser.FirstName + " " + objFamilyUser.LastName;
                        }
                    }

                }
            }
            return fullname;
        }

		public string GetUserRoleNameByEmail(string email)
		{
			string rolename = string.Empty;
			if (!string.IsNullOrEmpty(email))
			{
				using (var _ctx = new ChinmayaEntities())
				{
					var objUser = _ctx.Users.FirstOrDefault(x => x.Email == email);
					if (objUser != null)
					{
						rolename = _ctx.Roles.Where(r => r.Id == objUser.RoleId).Select(r => r.Name).FirstOrDefault(); ;
					}
				}
			}
			return rolename;
		}
	}
}

