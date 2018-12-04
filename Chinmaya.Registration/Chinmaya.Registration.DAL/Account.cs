using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chinmaya.DAL;
using Chinmaya.Registration.DAL;
using Chinmaya.Registration.Models;
using AutoMapper;

namespace Chinmaya.Registration.DAL
{
	public class Account
	{
        public bool IsEmailExists(string email)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                return _ctx.Users.Any(u => u.Email == email);
            }
        }

        public bool IsActiveUser(string email)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                return _ctx.Users.Any(u => u.Email == email && u.Status == true);
            }
        }

        public bool IsFamilyMember(string email)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                return _ctx.FamilyMembers.Any(u => u.Email == email);
            }
        }

        public bool AreAddressDetailsMatched(ContactDetails cd)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                bool isMatched = false;

                if (_ctx.Users.Any(u => u.HomePhone == cd.HomePhone))
                {
                    isMatched = true;
                    return isMatched;
                }

                var objUserList = _ctx.Users.Where(u => u.Address == cd.Address).ToList();
                foreach(User objUser in objUserList)
                {
                    if (objUser.City.ToLower() == cd.City.ToLower())
                    {
                        if (_ctx.Users.Any(u => u.StateId == cd.State))
                        {
                            isMatched = true;
                        }
                    }
                }

                return isMatched;
            }
        }

        public string GetUserIdByEmail(string email)
        {
            try
            {
                if (!string.IsNullOrEmpty(email))
                {
                    using (var _ctx = new ChinmayaEntities())
                    {
                        return _ctx.Users.SingleOrDefault(u => u.Email == email).Id;
                    }
                }
                return string.Empty;
            } catch(Exception)
            {
                return string.Empty;
            }
            
        }

        public List<SecurityQuestionsModel> GetSecurityQuestionsByEmail(string email)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                List<SecurityQuestionsModel> qlist = new List<SecurityQuestionsModel>();
                string userId = GetUserIdByEmail(email);
                if(!string.IsNullOrEmpty(userId))
                {
                    qlist = (from usq in _ctx.UserSecurityQuestions
                             join sq in _ctx.SecurityQuestions
                             on usq.SecurityQuestionId equals sq.Id
                             where usq.UserId == userId
                             select new SecurityQuestionsModel
                             {
                                 Id = sq.Id,
                                 Name = sq.Name,
                                 Value = usq.Answer
                             }).ToList();
                }
                
                return qlist;
            }
        }

        public string GetFamilyPrimaryAccountEmail(string email)
        {
            string result = string.Empty;
            using (var _ctx = new ChinmayaEntities())
            {
                var objFamilyMembers = _ctx.FamilyMembers.FirstOrDefault(x => x.Email == email);
                if (objFamilyMembers != null)
                {
                    var objUser =_ctx.Users.FirstOrDefault(x => x.Id == objFamilyMembers.UpdatedBy);
                    if (objUser != null)
                    {
                        return objUser.Email;
                    }
                }
                else
                {
                    if (IsEmailExists(email))
                    {
                        return email;
                    }
                }
            }
            return result;
        }

        public bool ResetUserPassword(ResetPasswordModel rpm)
        {
            if(!string.IsNullOrEmpty(rpm.Email) && !string.IsNullOrEmpty(rpm.Password))
            {
                using (var _ctx = new ChinmayaEntities())
                {
                    if(IsEmailExists(rpm.Email))
                    {
                        var objUser = _ctx.Users.FirstOrDefault(x => x.Email == rpm.Email);
                        objUser.Password = rpm.Password;
                        _ctx.Entry(objUser).State = System.Data.Entity.EntityState.Modified;
                        _ctx.SaveChanges();
                        return true;
                    }

                }
            }
            return false;
        }

        public void ChangeAccountType(string id)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                var user = _ctx.Users.Where(i => i.Id == id).FirstOrDefault();
                if (user != null)
                {
                    user.IsIndividual = false;
                    _ctx.SaveChanges();
                }
            }
        }

        public bool GetIsIndividual(string Id)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                return _ctx.Users.Where(r => r.Id == Id).Select(n => n.IsIndividual).FirstOrDefault();
            }
        }
    }
}
