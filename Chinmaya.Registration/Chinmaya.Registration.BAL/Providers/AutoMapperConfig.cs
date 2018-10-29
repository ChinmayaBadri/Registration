using AutoMapper;
using Chinmaya.DAL;
using Chinmaya.Registration.DAL;
using Chinmaya.Registration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinmaya.Registration.BAL.Providers
{
    public static class AutoMapperConfig
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<UserModel, User>();
				cfg.CreateMap<KeyValueModel, Role>();
				cfg.CreateMap<KeyValueModel, Gender>();
				cfg.CreateMap<KeyValueModel, Country>();
				cfg.CreateMap<KeyValueModel, State>();
				cfg.CreateMap<KeyValueModel, City>();
				cfg.CreateMap<KeyValueModel, SecurityQuestion>();
				cfg.CreateMap<KeyValueModel, AgeGroup>();
				cfg.CreateMap<KeyValueModel, Relationship>();
				cfg.CreateMap<KeyValueModel, Grade>();
				cfg.CreateMap<FamilyMemberModel, FamilyMember>();
			});
        }
    }
}
