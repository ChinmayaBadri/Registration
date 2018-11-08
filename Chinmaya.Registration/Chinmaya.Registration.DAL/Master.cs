using AutoMapper;
using Chinmaya.DAL;
using Chinmaya.Registration.Models;
using Chinmaya.Registration.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinmaya.Registration.DAL
{
    [DbConfigurationType(typeof(CodeConfig))] // point to the class that inherit from DbConfiguration
    public class Master
    {
        //Get all Employees
        public IEnumerable<KeyValueModel> GetMasterData(Utility.MasterType masterValue)
        {
            using (var _ctx = new ChinmayaEntities())
            {                
                switch (masterValue)
                {
                    case Utility.MasterType.ROLE:
                        return Mapper.Map<List<Role>, List<KeyValueModel>>(_ctx.Roles.ToList());
					case Utility.MasterType.GENDER:
						return Mapper.Map<List<Gender>, List<KeyValueModel>>(_ctx.Genders.ToList());
					case Utility.MasterType.COUNTRY:
						return Mapper.Map<List<Country>, List<KeyValueModel>>(_ctx.Countries.ToList());
					case Utility.MasterType.STATE:
						return Mapper.Map<List<State>, List<KeyValueModel>>(_ctx.States.ToList());
					case Utility.MasterType.CITY:
						return Mapper.Map<List<City>, List<KeyValueModel>>(_ctx.Cities.ToList());
					case Utility.MasterType.SECURITYQUESTIONS:
						return Mapper.Map<List<SecurityQuestion>, List<KeyValueModel>>(_ctx.SecurityQuestions.ToList());
					case Utility.MasterType.AGEGROUPID:
						return Mapper.Map<List<AgeGroup>, List<KeyValueModel>>(_ctx.AgeGroups.ToList());
					case Utility.MasterType.RELATIONSHIP:
						return Mapper.Map<List<Relationship>, List<KeyValueModel>>(_ctx.Relationships.ToList());
					case Utility.MasterType.GRADE:
						return Mapper.Map<List<Grade>, List<KeyValueModel>>(_ctx.Grades.ToList());
					case Utility.MasterType.WEEKDAY:
						return Mapper.Map<List<Weekday>, List<KeyValueModel>>(_ctx.Weekdays.ToList());
					case Utility.MasterType.FREQUENCY:
						return Mapper.Map<List<Frequency>, List<KeyValueModel>>(_ctx.Frequencies.ToList());
					case Utility.MasterType.ACCOUNTTYPE:
						return Mapper.Map<List<AccountType>, List<KeyValueModel>>(_ctx.AccountTypes.ToList());
					case Utility.MasterType.SESSION:
						return Mapper.Map<List<Session>, List<KeyValueModel>>(_ctx.Sessions.ToList());
					default:
                        return new List<KeyValueModel>();
                }            
            }
        }
    }
}
