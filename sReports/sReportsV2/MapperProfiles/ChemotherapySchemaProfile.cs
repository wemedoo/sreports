using AutoMapper;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchemaInstance;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataIn;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataOut;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DTO;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataIn;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataOut;
using sReportsV2.DTOs.DTOs.SmartOncology.ProgressNote.DataOut;
using sReportsV2.SqlDomain.Filter;
using System;
using System.Linq;

namespace sReportsV2.MapperProfiles
{
    public class ChemotherapySchemaProfile : Profile
    {
        public ChemotherapySchemaProfile()
        {
            CreateMap<ChemotherapySchemaDataIn, ChemotherapySchema>()
                .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.FromBase64String(src.RowVersion)))
                .ForMember(d => d.ChemotherapySchemaId, opt => opt.MapFrom(src => src.Id));

            CreateMap<EditGeneralPropertiesDataIn, ChemotherapySchema>()
                .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.FromBase64String(src.RowVersion)))
                .ForMember(d => d.ChemotherapySchemaId, opt => opt.MapFrom(src => src.Id));

            CreateMap<EditNameDataIn, ChemotherapySchema>()
                .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.FromBase64String(src.RowVersion)))
                .ForMember(d => d.ChemotherapySchemaId, opt => opt.MapFrom(src => src.Id));

            CreateMap<EditIndicationsDataIn, ChemotherapySchema>()
                .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.FromBase64String(src.RowVersion)))
                .ForMember(d => d.Indications, opt => opt.MapFrom(src => src.Indications))
                .ForMember(d => d.ChemotherapySchemaId, opt => opt.MapFrom(src => src.ChemotherapySchemaId));
            
            CreateMap<IndicationDataIn, Indication>()
                .ForMember(d => d.IndicationId, opt => opt.MapFrom(src => src.Id));

            CreateMap<MedicationDataIn, Medication>()
                .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.FromBase64String(src.RowVersion)))
                .ForMember(d => d.MedicationId, opt => opt.MapFrom(src => src.Id));

            CreateMap<EditMedicationDoseInBatchDataIn, Medication>()
                .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.FromBase64String(src.RowVersion)))
                .ForMember(d => d.MedicationId, opt => opt.MapFrom(src => src.MedicationId))
                .ForMember(d => d.MedicationDoses, opt => opt.MapFrom(src => src.Doses));

            CreateMap<MedicationDoseDataIn, MedicationDose>()
                .ForMember(d => d.MedicationDoseId, opt => opt.MapFrom(src => src.Id));

            CreateMap<MedicationDoseTimeDataIn, MedicationDoseTime>()
                .ForMember(d => d.MedicationDoseTimeId, opt => opt.MapFrom(src => src.Id));

            CreateMap<LiteratureReferenceDataIn, LiteratureReference>()
                .ForMember(d => d.LiteratureReferenceId, opt => opt.MapFrom(src => src.Id));

            CreateMap<ChemotherapySchema, ChemotherapySchemaDataOut>()
                .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.ToBase64String(src.RowVersion)))
                .ForMember(d => d.Indications, opt => opt.MapFrom(src => src.Indications.Where(x => !x.IsDeleted)))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.ChemotherapySchemaId));

            CreateMap<ChemotherapySchema, ChemotherapySchemaPreviewDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.ChemotherapySchemaId));

            CreateMap<Indication, IndicationDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.IndicationId));

            CreateMap<Medication, MedicationPreviewDataOut>()
                .ForMember(d => d.MedicationDoses, opt => opt.MapFrom(src => src.MedicationDoses.Where(x => !x.IsDeleted && x.IntervalId.HasValue).OrderBy(x => x.DayNumber)))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.MedicationId));

            CreateMap<Medication, SchemaTableMedicationDataOut>()
                .ForMember(d => d.MedicationDoses, opt => opt.MapFrom(src => src.MedicationDoses.Where(x => !x.IsDeleted && x.IntervalId.HasValue)))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.MedicationId));

            CreateMap<Medication, MedicationDataOut>()
                .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.ToBase64String(src.RowVersion)))
                .ForMember(d => d.MedicationDoses, opt => opt.MapFrom(src => src.MedicationDoses.Where(x => !x.IsDeleted)))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.MedicationId));

            CreateMap<MedicationDose, MedicationDoseDataOut>()
                .ForMember(d => d.StartsAt, opt => opt.MapFrom(src => src.GetStartTime()))
                .ForMember(d => d.MedicationDoseTimes, opt => opt.MapFrom(src => src.MedicationDoseTimes.Where(x => !x.IsDeleted)))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.MedicationDoseId));

            CreateMap<MedicationDose, MedicationDosePreviewDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.MedicationDoseId));

            CreateMap<MedicationDoseTime, MedicationDoseTimeDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.MedicationDoseTimeId));

            CreateMap<MedicationDoseDataOut, MedicationDoseInstanceDataOut>()
                .ForMember(d => d.DayNumber, opt => opt.MapFrom(src => src.DayNumber))
                .ForMember(d => d.Unit, opt => opt.MapFrom(src => src.Unit))
                .ForMember(d => d.UnitId, opt => opt.MapFrom(src => src.Unit.Id))
                .ForMember(d => d.MedicationDoseTimes, opt => opt.MapFrom(src => src.MedicationDoseTimes))
                .ForMember(d => d.StartsAt, opt => opt.MapFrom(src => src.StartsAt))
                .ForMember(d => d.IntervalId, opt => opt.MapFrom(src => src.IntervalId))
                .ForAllOtherMembers(d => d.Ignore());

            CreateMap<MedicationDoseTimeDataOut, MedicationDoseTimeInstanceDataOut>();

            CreateMap<LiteratureReference, LiteratureReferenceDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.LiteratureReferenceId));

            CreateMap<MedicationDoseType, MedicationDoseTypeDTO>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.MedicationDoseTypeId)); 

            CreateMap<MedicationDoseType, MedicationPreviewDoseTypeDTO>()
                .ForMember(d => d.StartAt, opt => opt.MapFrom(src => src.IntervalsList.FirstOrDefault() ?? ""))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.MedicationDoseTypeId));

            CreateMap<RouteOfAdministration, RouteOfAdministrationDTO>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.RouteOfAdministrationId))
                .ReverseMap();

            CreateMap<BodySurfaceCalculationFormula, BodySurfaceCalculationFormulaDTO>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.BodySurfaceCalculationFormulaId));

            CreateMap<Unit, UnitDTO>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.UnitId))
                .ReverseMap();

            CreateMap<ChemotherapySchemaFilterDataIn, ChemotherapySchemaFilter>();

            CreateMap<ChemotherapySchemaInstanceDataIn, ChemotherapySchemaInstance>()
                .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.FromBase64String(src.RowVersion)))
                .ForMember(d => d.ChemotherapySchemaInstanceId, opt => opt.MapFrom(src => src.Id));

            CreateMap<MedicationInstanceDataIn, MedicationInstance>()
                .ForMember(d => d.MedicationInstanceId, opt => opt.MapFrom(src => src.Id)); 

            CreateMap<MedicationDoseInstanceDataIn, MedicationDoseInstance>()
                .ForMember(d => d.MedicationDoseInstanceId, opt => opt.MapFrom(src => src.Id)); 

            CreateMap<MedicationDoseTimeInstanceDataIn, MedicationDoseTimeInstance>()
                .ForMember(d => d.MedicationDoseTimeInstanceId, opt => opt.MapFrom(src => src.Id));

            CreateMap<ChemotherapySchemaInstance, ChemotherapySchemaInstanceDataOut>()
                .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.ToBase64String(src.RowVersion)))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.ChemotherapySchemaInstanceId));

            CreateMap<ChemotherapySchemaInstance, ChemotherapySchemaInstancePreviewDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.ChemotherapySchemaInstanceId));

            CreateMap<ChemotherapySchemaInstanceFilterDataIn, ChemotherapySchemaInstanceFilter>();

            CreateMap<ChemotherapySchemaInstanceVersion, ChemotherapySchemaInstanceVersionDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.ChemotherapySchemaInstanceVersionId));

            CreateMap<MedicationReplacement, MedicationReplacementDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.MedicationReplacementId));

            CreateMap<MedicationInstance, SchemaTableMedicationInstanceDataOut>()
                .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.ToBase64String(src.RowVersion)))
                .ForMember(d => d.MedicationDoses, opt => opt.MapFrom(src => src.MedicationDoses.Where(x => !x.IsDeleted)))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.MedicationInstanceId));

            CreateMap<MedicationInstance, MedicationInstanceDataOut>()
                .ForMember(d => d.MedicationDoses, opt => opt.MapFrom(src => src.MedicationDoses.Where(x => !x.IsDeleted)))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.MedicationInstanceId));

            CreateMap<MedicationInstance, MedicationInstancePreviewDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.MedicationInstanceId)); 
            
            CreateMap<MedicationDoseInstance, MedicationDoseInstanceDataOut>()
                .ForMember(d => d.MedicationDoseTimes, opt => opt.MapFrom(src => src.MedicationDoseTimes.Where(x => !x.IsDeleted)))
                .ForMember(d => d.StartsAt, opt => opt.MapFrom(src => src.GetStartTime()))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.MedicationInstanceId));

            CreateMap<MedicationDoseTimeInstance, MedicationDoseTimeInstanceDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.MedicationDoseTimeInstanceId)); 
        }
    }
}