using AutoMapper;
using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.DTOs.DocumentProperties.DataIn;
using sReportsV2.DTOs.DocumentProperties.DataOut;

namespace sReportsV2.MapperProfiles
{
    public class DocumentPropertiesProfile : Profile
    {
        public DocumentPropertiesProfile()
        {
            /*DATA Out*/
            CreateMap<DocumentProperties, DocumentPropertiesDataOut>();
            CreateMap<DocumentPurpose, DocumentPurposeDataOut>();
            CreateMap<DocumentGeneralPurpose, DocumentGeneralPurposeDataOut>();
            CreateMap<DocumentScopeOfValidity, DocumentScopeOfValidityDataOut>();
            CreateMap<DocumentClinicalContext, DocumentClinicalContextDataOut>();
            CreateMap<DocumentClassDataOut, DocumentClass>();

            /*DATA IN*/
            CreateMap<DocumentPropertiesDataIn, DocumentProperties>();
            CreateMap<DocumentPurposeDataIn, DocumentPurpose>();
            CreateMap<DocumentGeneralPurposeDataIn, DocumentGeneralPurpose>();
            CreateMap<DocumentScopeOfValidityDataIn, DocumentScopeOfValidity>();
            CreateMap<DocumentClinicalContextDataIn, DocumentClinicalContext>();
            CreateMap<DocumentClassDataIn, DocumentClass>();


            CreateMap<DocumentPropertiesDataIn, DocumentPropertiesDataOut>();
            CreateMap<DocumentPurposeDataIn, DocumentPurposeDataOut>();
            CreateMap<DocumentGeneralPurposeDataIn, DocumentGeneralPurposeDataOut>();
            CreateMap<DocumentScopeOfValidityDataIn, DocumentScopeOfValidityDataOut>();
            CreateMap<DocumentClinicalContextDataIn, DocumentClinicalContextDataOut>();
            CreateMap<DocumentClassDataIn, DocumentClassDataOut>();


        }
    }
}