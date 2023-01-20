using AutoMapper;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.Domain.Sql.Entities.SmartOncologyPatient;
using sReportsV2.DTOs.DTOs.SmartOncologyPatient.DataIn;
using sReportsV2.DTOs.DTOs.SmartOncologyPatient.DataOut;
using sReportsV2.SqlDomain.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.MapperProfiles
{
    public class SmartOncologyPatientProfile : Profile
    {
        public SmartOncologyPatientProfile()
        {
            CreateMap<SmartOncologyPatientDataIn, SmartOncologyPatient>()
                .ForMember(o => o.SmartOncologyPatientId, opt => opt.MapFrom(src => src.Id))
                .ForMember(o => o.Active, opt => opt.MapFrom(src => src.Activity))
                .ForMember(o => o.Name, opt => opt.MapFrom(src => new Domain.Sql.Entities.Common.Name()
                {
                    Family = src.FamilyName,
                    Given = src.Name
                }))
                .ForMember(o => o.Contact, opt => opt.MapFrom(src => src.ContactPerson))
                .ForMember(o => o.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(o => o.MultipleBirth, opt => opt.MapFrom(src => new MultipleBirth()
                {
                    isMultipleBorn = src.MultipleBirth,
                    Number = src.MultipleBirthNumber
                }))
                .ForMember(o => o.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(o => o.Identifiers, opt => opt.MapFrom(src => src.Identifiers))
                .ForMember(o => o.Communications, opt => opt.MapFrom(src => src.Communications))

                .ForMember(o => o.IdentificationNumber, opt => opt.MapFrom(src => src.IdentificationNumber))
                .ForMember(o => o.Allergies, opt => opt.MapFrom(src => src.Allergies))
                .ForMember(o => o.PatientInformedFor, opt => opt.MapFrom(src => src.PatientInformedFor))
                .ForMember(o => o.PatientInformedBy, opt => opt.MapFrom(src => src.PatientInformedBy))
                .ForMember(o => o.PatientInfoSignedOn, opt => opt.MapFrom(src => src.PatientInfoSignedOn))
                .ForMember(o => o.CopyDeliveredOn, opt => opt.MapFrom(src => src.CopyDeliveredOn))
                .ForMember(o => o.CapabilityToWork, opt => opt.MapFrom(src => src.CapabilityToWork))
                .ForMember(o => o.DesireToHaveChildren, opt => opt.MapFrom(src => src.DesireToHaveChildren))
                .ForMember(o => o.FertilityConservation, opt => opt.MapFrom(src => src.FertilityConservation))
                .ForMember(o => o.SemenCryopreservation, opt => opt.MapFrom(src => src.SemenCryopreservation))
                .ForMember(o => o.EggCellCryopreservation, opt => opt.MapFrom(src => src.EggCellCryopreservation))
                .ForMember(o => o.SexualHealthAddressed, opt => opt.MapFrom(src => src.SexualHealthAddressed))
                .ForMember(o => o.Contraception, opt => opt.MapFrom(src => src.Contraception))
                .ForMember(o => o.ClinicalTrials, opt => opt.MapFrom(src => src.ClinicalTrials))
                .ForMember(o => o.PreviousTreatment, opt => opt.MapFrom(src => src.PreviousTreatment))
                .ForMember(o => o.TreatmentInCantonalHospitalGraubunden, opt => opt.MapFrom(src => src.TreatmentInCantonalHospitalGraubunden))
                .ForMember(o => o.HistoryOfOncologicalDisease, opt => opt.MapFrom(src => src.HistoryOfOncologicalDisease))
                .ForMember(o => o.HospitalOrPraxisOfPreviousTreatments, opt => opt.MapFrom(src => src.HospitalOrPraxisOfPreviousTreatments))
                .ForMember(o => o.DiseaseContextAtInitialPresentation, opt => opt.MapFrom(src => src.DiseaseContextAtInitialPresentation))
                .ForMember(o => o.StageAtInitialPresentation, opt => opt.MapFrom(src => src.StageAtInitialPresentation))
                .ForMember(o => o.DiseaseContextAtCurrentPresentation, opt => opt.MapFrom(src => src.DiseaseContextAtCurrentPresentation))
                .ForMember(o => o.StageAtCurrentPresentation, opt => opt.MapFrom(src => src.StageAtCurrentPresentation))
                .ForMember(o => o.Anatomy, opt => opt.MapFrom(src => src.Anatomy))
                .ForMember(o => o.Morphology, opt => opt.MapFrom(src => src.Morphology))
                .ForMember(o => o.TherapeuticContext, opt => opt.MapFrom(src => src.TherapeuticContext))
                .ForMember(o => o.ChemotherapyType, opt => opt.MapFrom(src => src.ChemotherapyType))
                .ForMember(o => o.ChemotherapyCourse, opt => opt.MapFrom(src => src.ChemotherapyCourse))
                .ForMember(o => o.ChemotherapyCycle, opt => opt.MapFrom(src => src.ChemotherapyCycle))
                .ForMember(o => o.FirstDayOfChemotherapy, opt => opt.MapFrom(src => src.FirstDayOfChemotherapy))
                .ForMember(o => o.ConsecutiveChemotherapyDays, opt => opt.MapFrom(src => src.ConsecutiveChemotherapyDays));

            CreateMap<SmartOncologyPatient, SmartOncologyPatientDataOut>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.SmartOncologyPatientId.ToString()))
                .ForMember(o => o.Activity, opt => opt.MapFrom(src => src.Active))
                .ForMember(o => o.Name, opt => opt.MapFrom(src => src.Name.Given))
                .ForMember(o => o.FamilyName, opt => opt.MapFrom(src => src.Name.Family))
                .ForMember(o => o.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(o => o.MultipleBirth, opt => opt.MapFrom(src => src.MultipleBirth.isMultipleBorn))
                .ForMember(o => o.MultipleBirthNumber, opt => opt.MapFrom(src => src.MultipleBirth.Number))
                .ForMember(o => o.ContactGender, opt => opt.MapFrom(src => src.Contact.Gender))
                .ForMember(o => o.ContactName, opt => opt.MapFrom(src => src.Contact.Name.Given))
                .ForMember(o => o.ContactFamily, opt => opt.MapFrom(src => src.Contact.Name.Family))
                .ForMember(o => o.Relationship, opt => opt.MapFrom(src => src.Contact.Relationship))
                .ForMember(o => o.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(o => o.ContactTelecoms, opt => opt.MapFrom(src => src.Contact.Telecoms))
                .ForMember(o => o.ContactAddress, opt => opt.MapFrom(src => src.Contact.Address))
                .ForMember(o => o.Communications, opt => opt.MapFrom(src => src.Communications))
                .ForMember(o => o.Identifiers, opt => opt.MapFrom(src => src.Identifiers))

                .ForMember(o => o.IdentificationNumber, opt => opt.MapFrom(src => src.IdentificationNumber))
                .ForMember(o => o.Allergies, opt => opt.MapFrom(src => src.Allergies))
                .ForMember(o => o.PatientInformedFor, opt => opt.MapFrom(src => src.PatientInformedFor))
                .ForMember(o => o.PatientInformedBy, opt => opt.MapFrom(src => src.PatientInformedBy))
                .ForMember(o => o.PatientInfoSignedOn, opt => opt.MapFrom(src => src.PatientInfoSignedOn))
                .ForMember(o => o.CopyDeliveredOn, opt => opt.MapFrom(src => src.CopyDeliveredOn))
                .ForMember(o => o.CapabilityToWork, opt => opt.MapFrom(src => src.CapabilityToWork))
                .ForMember(o => o.DesireToHaveChildren, opt => opt.MapFrom(src => src.DesireToHaveChildren))
                .ForMember(o => o.FertilityConservation, opt => opt.MapFrom(src => src.FertilityConservation))
                .ForMember(o => o.SemenCryopreservation, opt => opt.MapFrom(src => src.SemenCryopreservation))
                .ForMember(o => o.EggCellCryopreservation, opt => opt.MapFrom(src => src.EggCellCryopreservation))
                .ForMember(o => o.SexualHealthAddressed, opt => opt.MapFrom(src => src.SexualHealthAddressed))
                .ForMember(o => o.Contraception, opt => opt.MapFrom(src => src.Contraception))
                //.ForMember(o => o.ClinicalTrials, opt => opt.MapFrom(src => src.ClinicalTrials))
                .ForMember(o => o.PreviousTreatment, opt => opt.MapFrom(src => src.PreviousTreatment))
                .ForMember(o => o.TreatmentInCantonalHospitalGraubunden, opt => opt.MapFrom(src => src.TreatmentInCantonalHospitalGraubunden))
                .ForMember(o => o.HistoryOfOncologicalDisease, opt => opt.MapFrom(src => src.HistoryOfOncologicalDisease))
                .ForMember(o => o.HospitalOrPraxisOfPreviousTreatments, opt => opt.MapFrom(src => src.HospitalOrPraxisOfPreviousTreatments))
                .ForMember(o => o.DiseaseContextAtInitialPresentation, opt => opt.MapFrom(src => src.DiseaseContextAtInitialPresentation))
                .ForMember(o => o.StageAtInitialPresentation, opt => opt.MapFrom(src => src.StageAtInitialPresentation))
                .ForMember(o => o.DiseaseContextAtCurrentPresentation, opt => opt.MapFrom(src => src.DiseaseContextAtCurrentPresentation))
                .ForMember(o => o.StageAtCurrentPresentation, opt => opt.MapFrom(src => src.StageAtCurrentPresentation))
                .ForMember(o => o.Anatomy, opt => opt.MapFrom(src => src.Anatomy))
                .ForMember(o => o.Morphology, opt => opt.MapFrom(src => src.Morphology))
                .ForMember(o => o.TherapeuticContext, opt => opt.MapFrom(src => src.TherapeuticContext))
                .ForMember(o => o.ChemotherapyType, opt => opt.MapFrom(src => src.ChemotherapyType))
                .ForMember(o => o.ChemotherapyCourse, opt => opt.MapFrom(src => src.ChemotherapyCourse))
                .ForMember(o => o.ChemotherapyCycle, opt => opt.MapFrom(src => src.ChemotherapyCycle))
                .ForMember(o => o.FirstDayOfChemotherapy, opt => opt.MapFrom(src => src.FirstDayOfChemotherapy))
                .ForMember(o => o.ConsecutiveChemotherapyDays, opt => opt.MapFrom(src => src.ConsecutiveChemotherapyDays))
                .ForAllOtherMembers(opts => opts.Ignore());
            ;

            CreateMap<SmartOncologyPatient, SmartOncologyPatientPreviewDataOut>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.SmartOncologyPatientId.ToString()))
                .ForMember(o => o.FirstName, opt => opt.MapFrom(src => src.Name.Given))
                .ForMember(o => o.LastName, opt => opt.MapFrom(src => src.Name.Family))
                .ForMember(o => o.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(o => o.Gender, opt => opt.MapFrom(src => src.Gender));

            CreateMap<SmartOncologyPatientFilterDataIn, SmartOncologyPatientFilter>();
        }
    }
}