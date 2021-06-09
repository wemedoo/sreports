using AutoMapper;
using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using sReportsV2.DTOs.DTOs.GlobalThesaurusUser.DataIn;
using sReportsV2.DTOs.DTOs.GlobalThesaurusUser.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.MapperProfiles
{
    public class GlobalThesaurusUserProfile : Profile
    {
        public GlobalThesaurusUserProfile() 
        {
            CreateMap<GlobalThesaurusUser, GlobalThesaurusUserDataIn>()
             .ReverseMap();

            CreateMap<GlobalThesaurusUser, GlobalThesaurusUserDataOut>()
             .ReverseMap();
        }
    }
}