using AutoMapper;
using sReportsV2.Api.DTOs;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.Encounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Api.MapperProfiles
{
    public class EncounterProfile : Profile
    {
        public EncounterProfile()
        {
            /*CreateMap<EncounterFilterDataIn, EncounterFhirFilter>();

            CreateMap<EncounterEntity, Encounter>()
                .ForMember(org => org.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(enc => enc.Status, opt => opt.MapFrom(src => EncounterStatus.Finished))
                .ForMember(enc => enc.Class, opt => opt.MapFrom(src => new Coding(O40MtConstants.O40Mt, src.Class)))
                .ForMember(enc => enc.Type, opt => opt.MapFrom(src => new List<CodeableConcept>() { new CodeableConcept(O40MtConstants.O40Mt, src.Type, null) }))
                .ForMember(enc => enc.EpisodeOfCare, opt => opt.MapFrom(src => new List<ResourceReference>() { new ResourceReference() { Reference = src.EpisodeOfCareId } }))
                .ForAllOtherMembers(opts => opts.Ignore());*/
        }
    }
}
