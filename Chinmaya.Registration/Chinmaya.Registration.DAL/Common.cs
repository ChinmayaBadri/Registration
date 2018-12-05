using AutoMapper;
using Chinmaya.DAL;
using Chinmaya.Registration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinmaya.Registration.DAL
{
    public class Common
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

		public int GetCountryId(string name)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				return _ctx.Countries.Where(c => c.Name == name).FirstOrDefault().Id;
			}
		}

		public EmailTemplateModel GetEmailTemplateByID(int id)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<EmailTemplate, EmailTemplateModel>();
                });
                IMapper mapper = config.CreateMapper();
                EmailTemplateModel etm = new EmailTemplateModel();
                return mapper.Map(_ctx.EmailTemplates.FirstOrDefault(et => et.ID == id), etm);
            }
        }
    }
}
