using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chinmaya.DAL;
using Chinmaya.Registration.DAL;
using Chinmaya.Registration.Models;

namespace Chinmaya.Registration.DAL
{
	public class Account
	{
		public List<KeyValueModel> GetStateName(int id)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				return _ctx.States.Where(s => s.CountryID == id).Select(x => new KeyValueModel
				{
					Id = x.Id,
					Name = x.Name
				}).ToList();
			}
		}

		public List<KeyValueModel> GetCityName(int id)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				return _ctx.Cities.Where(s => s.StateID == id).Select(x => new KeyValueModel
				{
					Id = x.Id,
					Name = x.Name
				}).ToList();
			}
		}

        public bool IsEmailExists(string email)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                return _ctx.Users.Any(u => u.Email == email)
                        || _ctx.FamilyMembers.Any(u => u.Email == email);
            }
        }

        public bool AreAddressDetailsMatched(ContactDetails cd)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                if (_ctx.Users.Any(u => u.HomePhone == cd.HomePhone))
                {
                    return true;
                }
                if (_ctx.Users.Any(u => u.Address.ToLower() == cd.Address.ToLower()))
                {
                    if (_ctx.Users.Any(u => u.City.ToLower() == cd.City.ToLower()))
                    {
                        if (_ctx.Users.Any(u => u.StateId == cd.State))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        /*public List<KeyValueModel> GetGender()
		{
			using (var _ctx = new ChinmayaEntities())
			{
				return _ctx.Genders.Select(x => new KeyValueModel
				{
					Id = x.Id,
					Name = x.Name
				}).ToList();
			}
		}

		public List<KeyValueModel> GetCountryList()
		{
			using (var _ctx = new ChinmayaEntities())
			{
				return _ctx.Countries.Select(x => new KeyValueModel
				{
					Id = x.Id,
					Name = x.Name
				}).ToList();
			}
		}

		public List<KeyValueModel> GetStateList(int CountryId)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				return _ctx.States.Where(s => s.Id == CountryId).Select(x => new KeyValueModel
				{
					Id = x.Id,
					Name = x.Name
				}).ToList();
			}
		}

		public List<KeyValueModel> GetCityList(int StateId)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				return _ctx.Cities.Where(c => c.Id == StateId).Select(x => new KeyValueModel
				{
					Id = x.Id,
					Name = x.Name
				}).ToList();
			}
		}*/

    }
}
