using AutoMapper;
using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.DTOs.DocumentProperties.DataIn;
using sReportsV2.DTOs.DocumentProperties.DataOut;
using sReportsV2.Models.Common;

namespace sReportsV2.MapperProfiles
{
    public class DocumentPropertiesProfile : Profile
    {
        public DocumentPropertiesProfile()
        {
            CreateMap<Domain.Entities.Common.Version, VersionViewModel>()
                .ReverseMap();

            /*DATA Out*/
            CreateMap<DocumentProperties, DocumentPropertiesDataOut>();
            CreateMap<DocumentPurpose, DocumentPurposeDataOut>();
            CreateMap<DocumentGeneralPurpose, DocumentGeneralPurposeDataOut>();
            CreateMap<DocumentScopeOfValidity, DocumentScopeOfValidityDataOut>();
            CreateMap<DocumentClinicalContext, DocumentClinicalContextDataOut>();

            /*DATA IN*/
            CreateMap<DocumentPropertiesDataIn, DocumentProperties>();
            CreateMap<DocumentPurposeDataIn, DocumentPurpose>();
            CreateMap<DocumentGeneralPurposeDataIn, DocumentGeneralPurpose>();
            CreateMap<DocumentScopeOfValidityDataIn, DocumentScopeOfValidity>();
            CreateMap<DocumentClinicalContextDataIn, DocumentClinicalContext>();

        }
    }
}