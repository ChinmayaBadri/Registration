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
        /// <summary>
        /// Check user email exists or not
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> true or false </returns>
        public bool IsEmailExists(string email)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                return _ctx.Users.Any(u => u.Email == email);
            }
        }

        /// <summary>
        /// checks whether user is active or inactive user
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> true or false </returns>
        public bool IsActiveUser(string email)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                return _ctx.Users.Any(u => u.Email == email && u.Status == true);
            }
        }

        /// <summary>
        /// checks is user family member or not
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> true or false </returns>
        public bool IsFamilyMember(string email)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                return _ctx.FamilyMembers.Any(u => u.Email == email);
            }
        }

        /// <summary>
        /// checks user addess or home phone is matched with any other account's address or home phone
        /// </summary>
        /// <param name="cd"> contact details model </param>
        /// <returns> true or false </returns>
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

        /// <summary>
        /// gets user id by email
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> User id </returns>
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

        /// <summary>
        /// Gets security questions by email
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> list of security questions model </returns>
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

        /// <summary>
        /// Get family primary account email id
        /// </summary>
        /// <param name="email"> user email </param>
        /// <returns> primary account email id </returns>
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

        /// <summary>
        /// update user password
        /// </summary>
        /// <param name="rpm">Reset Password Model</param>
        /// <returns> true or false </returns>
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

        /// <summary>
        /// user account type will changed
        /// </summary>
        /// <param name="id"> user id </param>
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

        /// <summary>
        /// gets user account type
        /// </summary>
        /// <param name="Id"> user id </param>
        /// <returns> true or false </returns>
        public bool GetIsIndividual(string Id)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                return _ctx.Users.Where(r => r.Id == Id).Select(n => n.IsIndividual).FirstOrDefault();
            }
        }
    }
}
