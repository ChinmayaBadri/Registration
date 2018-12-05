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
        /// <summary>
        /// gets states
        /// </summary>
        /// <param name="id"> country id </param>
        /// <returns> list of states </returns>
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

        /// <summary>
        /// gets cities
        /// </summary>
        /// <param name="id"> state id </param>
        /// <returns> list of cities </returns>
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

        /// <summary>
        /// gets country id
        /// </summary>
        /// <param name="name"> country name </param>
        /// <returns> country id </returns>
		public int GetCountryId(string name)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				return _ctx.Countries.Where(c => c.Name == name).FirstOrDefault().Id;
			}
		}

        /// <summary>
        /// gets email template by template id
        /// </summary>
        /// <param name="id"> template id </param>
        /// <returns> email template model </returns>
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
