using AutoMapper;
using sReportsV2.Api.DTOs.FormInstance.DataIn;
using sReportsV2.Domain.Entities.FormInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Api.MapperProfiles
{
    public class FormInstanceProfile : Profile
    {
        public FormInstanceProfile() 
        {
            //CreateMap<FormInstanceFilterDataIn, FormInstanceFhirFilter>();
        }
    }
}
