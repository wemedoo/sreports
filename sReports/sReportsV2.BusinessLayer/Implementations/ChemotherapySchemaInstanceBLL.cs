using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Exceptions;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.SmartOncologyEnums;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchemaInstance;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataIn;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DTO;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataIn;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataOut;
using sReportsV2.DTOs.DTOs.SmartOncology.ProgressNote.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.SqlDomain.Filter;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class ChemotherapySchemaInstanceBLL : IChemotherapySchemaInstanceBLL
    {
        private readonly IChemotherapySchemaInstanceDAL chemotherapySchemaInstanceDAL;
        private readonly IChemotherapySchemaInstanceHistoryDAL chemotherapySchemaInstanceHistoryDAL;
        private readonly IMedicationInstanceDAL medicationInstanceDAL;
        private readonly IMedicationReplacementDAL medicationReplacementDAL;
        private readonly IMedicationDoseInstanceDAL medicationDoseInstanceDAL;
        private readonly IUserDAL userDAL;

        public ChemotherapySchemaInstanceBLL(IChemotherapySchemaInstanceDAL chemotherapySchemaInstanceDAL, IChemotherapySchemaInstanceHistoryDAL chemotherapySchemaInstanceHistoryDAL, IMedicationInstanceDAL medicationInstanceDAL, IMedicationReplacementDAL medicationReplacementDAL, IMedicationDoseInstanceDAL medicationDoseInstanceDAL, IUserDAL userDAL)
        {
            this.chemotherapySchemaInstanceDAL = chemotherapySchemaInstanceDAL;
            this.chemotherapySchemaInstanceHistoryDAL = chemotherapySchemaInstanceHistoryDAL;
            this.medicationInstanceDAL = medicationInstanceDAL;
            this.medicationReplacementDAL = medicationReplacementDAL;
            this.medicationDoseInstanceDAL = medicationDoseInstanceDAL;
            this.userDAL = userDAL;
        }

        public void Delete(int id)
        {
            chemotherapySchemaInstanceDAL.Delete(id);
        }

        public ChemotherapySchemaInstanceDataOut GetById(int id)
        {
            ChemotherapySchemaInstance chemotherapySchemaInstance = chemotherapySchemaInstanceDAL.GetSchemaInstance(id);
            if (chemotherapySchemaInstance == null) throw new NullReferenceException();

            ChemotherapySchemaInstanceDataOut chemotherapySchemaInstanceDataOut = Mapper.Map<ChemotherapySchemaInstanceDataOut>(chemotherapySchemaInstance);
            return chemotherapySchemaInstanceDataOut;
        }

        public ChemotherapySchemaInstanceDataOut GetSchemaInstance(int id)
        {
            ChemotherapySchemaInstance chemotherapySchemaInstance = chemotherapySchemaInstanceDAL.GetSchemaInstance(id);
            if (chemotherapySchemaInstance == null) throw new NullReferenceException();

            ChemotherapySchemaInstanceDataOut chemotherapySchemaInstanceDataOut = Mapper.Map<ChemotherapySchemaInstanceDataOut>(chemotherapySchemaInstance);
            chemotherapySchemaInstanceDataOut.SchemaTableData = GetSchemaTableData(chemotherapySchemaInstance);
            chemotherapySchemaInstanceDataOut.Patient.ClinicalTrials = Mapper.Map<List<ClinicalTrialDTO>>(userDAL.GetlClinicalTrialByIds(chemotherapySchemaInstance.Patient.GetClinicalTrialIds()));
            return chemotherapySchemaInstanceDataOut;
        }

        public ResourceCreatedDTO InsertOrUpdate(ChemotherapySchemaInstanceDataIn dataIn, UserCookieData userCookieData)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            ChemotherapySchemaInstance chemotherapySchemaInstance = Mapper.Map<ChemotherapySchemaInstance>(dataIn);
            ChemotherapySchemaInstance chemotherapySchemaInstanceDb = chemotherapySchemaInstanceDAL.GetById(dataIn.Id);

            string actionName;
            if (chemotherapySchemaInstanceDb == null)
            {
                chemotherapySchemaInstanceDb = chemotherapySchemaInstance;
                chemotherapySchemaInstanceDb.State = InstanceState.Active;
                chemotherapySchemaInstance.CreatorId = userCookieData.Id;
                actionName = "created";
            }
            else
            {
                chemotherapySchemaInstanceDb.Copy(chemotherapySchemaInstance);
                actionName = "updated";
            }

            chemotherapySchemaInstanceDAL.InsertOrUpdate(chemotherapySchemaInstanceDb);
            SaveChemotherapySchemaInstanceAction(chemotherapySchemaInstanceDb.ChemotherapySchemaInstanceId, userCookieData.Id, ChemotherapySchemaInstanceActionType.SaveInstance, $"ChemotherapySchemaInstance [{chemotherapySchemaInstanceDAL.GetName(chemotherapySchemaInstanceDb.ChemotherapySchemaInstanceId)}] is {actionName}.");

            return new ResourceCreatedDTO() { Id = chemotherapySchemaInstanceDb.ChemotherapySchemaInstanceId.ToString(), RowVersion = Convert.ToBase64String(chemotherapySchemaInstanceDb.RowVersion) };
        }

        public PaginationDataOut<ChemotherapySchemaInstancePreviewDataOut, DataIn> ReloadTable(ChemotherapySchemaInstanceFilterDataIn dataIn)
        {
            Ensure.IsNotNull(dataIn, nameof(dataIn));

            ChemotherapySchemaInstanceFilter filterData = Mapper.Map<ChemotherapySchemaInstanceFilter>(dataIn);

            List<ChemotherapySchemaInstance> chemotherapySchemasFiltered = chemotherapySchemaInstanceDAL.GetAll(filterData);
            PaginationDataOut<ChemotherapySchemaInstancePreviewDataOut, DataIn> result = new PaginationDataOut<ChemotherapySchemaInstancePreviewDataOut, DataIn>()
            {
                Count = (int)chemotherapySchemaInstanceDAL.GetAllFilteredCount(filterData),
                Data = Mapper.Map<List<ChemotherapySchemaInstancePreviewDataOut>>(chemotherapySchemasFiltered),
                DataIn = dataIn
            };

            return result;
        }

        public MedicationDoseInstanceDataOut UpdateMedicationDoseInstance(MedicationDoseInstanceDataIn dataIn, UserCookieData userCookieData)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            MedicationDoseInstance medicationDoseInstance = Mapper.Map<MedicationDoseInstance>(dataIn);
            MedicationDoseInstance medicationDoseInstanceDb = medicationDoseInstanceDAL.GetById(dataIn.Id);

            string actionName;
            if (medicationDoseInstanceDb == null)
            {
                medicationDoseInstanceDb = medicationDoseInstance;
                actionName = "created";
            }
            else
            {
                medicationDoseInstanceDb.Copy(medicationDoseInstance);
                actionName = "updated";
            }
            IsConcurrencyMedicationInstanceViolated(dataIn.MedicationInstanceId, dataIn.RowVersion);

            medicationDoseInstanceDAL.InsertOrUpdate(medicationDoseInstanceDb);
            SaveChemotherapySchemaInstanceAction(dataIn.ChemotherapySchemaInstanceId, userCookieData.Id, ChemotherapySchemaInstanceActionType.SaveDose, $"Medication dose instance (day {medicationDoseInstanceDb.DayNumber}) for {dataIn.MedicationName} is {actionName}.");

            return GetMedicationDoseInstanceDataOut(medicationDoseInstanceDb, dataIn.RowVersion);
        }

        public void DeleteDose(EditMedicationDoseDataIn dataIn, UserCookieData userCookieData)
        {
            IsConcurrencyMedicationInstanceViolated(dataIn.MedicationId, dataIn.RowVersion);
            int doseDeletedId = medicationDoseInstanceDAL.Delete(dataIn.Id);
            if(doseDeletedId > 0)
            {
                SaveChemotherapySchemaInstanceAction(dataIn.ChemotherapySchemaInstanceId, userCookieData.Id, ChemotherapySchemaInstanceActionType.DeleteDose, $"Medication dose instance (day {dataIn.DayNumber}) for {dataIn.MedicationName} is deleted.");
            }
        }

        public SchemaTableDataOut DelayDose(DelayDoseDataIn dataIn, UserCookieData userCookieData)
        {
            ChemotherapySchemaInstanceVersion newChemotherapySchemaInstanceVersion = new ChemotherapySchemaInstanceVersion()
            {
                DelayFor = dataIn.DelayFor,
                FirstDelayDay = dataIn.DayNumber,
                ReasonForDelay = dataIn.ReasonForDelay,
            };

            SaveChemotherapySchemaInstanceAction(dataIn.ChemotherapySchemaInstanceId, userCookieData.Id, ChemotherapySchemaInstanceActionType.DelayDose, $"Day number {dataIn.DayNumber} is delayed for {dataIn.DelayFor} days. Reason for delay: {dataIn.ReasonForDelay}.", newChemotherapySchemaInstanceVersion);

            return GetSchemaTableData(dataIn.ChemotherapySchemaInstanceId);
        }

        public ChemotherapySchemaInstanceHistoryDataOut ViewHistoryOfDayDose(DelayDoseHistoryDataIn dataIn)
        {
            var history = Mapper.Map<List<ChemotherapySchemaInstanceVersionDataOut>>(chemotherapySchemaInstanceHistoryDAL.ViewHistoryOfDayDose(dataIn.ChemotherapySchemaInstanceId, dataIn.DayNumber));

            var data = new ChemotherapySchemaInstanceHistoryDataOut()
            {
                FirstDelayDay = dataIn.DayNumber,
                StartDayDate = dataIn.StartDate.AppendDays(dataIn.DayNumber),
                History = history
            };

            data.CalculateDelayedDateByVersions();

            return data;
        }

        public void DeleteMedicationInstance(DeleteMedicationInstanceDataIn dataIn, UserCookieData userCookieData)
        {
            IsConcurrencyViolated(dataIn.ChemotherapySchemaInstanceId, dataIn.RowVersion);
            IsConcurrencyMedicationInstanceViolated(dataIn.Id, dataIn.RowVersionMedication);
            if (medicationReplacementDAL.DoesMedicationParticipateInAnyReplacment(dataIn.Id))
            {
                throw new DuplicateException($"Medication [{string.Join(",", medicationInstanceDAL.GetNameByIds(new List<int> { dataIn.Id }))}] cannot be deleted because it is used as medication replacment.");
            }
            else
            {
                int medicationDeletedId = medicationInstanceDAL.Delete(dataIn.Id);
                if (medicationDeletedId > 0)
                {
                    SaveChemotherapySchemaInstanceAction(dataIn.ChemotherapySchemaInstanceId, userCookieData.Id, ChemotherapySchemaInstanceActionType.DeleteMedication, $"Medication [{string.Join(",", medicationInstanceDAL.GetNameByIds(new List<int> { dataIn.Id }))}] is deleted from schema instance.");
                }
            }
        }

        public ChemotherapySchemaResourceCreatedDTO UpdateMedicationInstance(MedicationInstanceDataIn dataIn, UserCookieData userCookieData)
        {
            MedicationInstance medicationInstance = Mapper.Map<MedicationInstance>(dataIn);
            MedicationInstance medicationInstanceDb = medicationInstanceDAL.GetById(dataIn.Id);

            if (medicationInstanceDb == null)
            {
                medicationInstanceDb = medicationInstance;

                medicationInstanceDAL.InsertOrUpdate(medicationInstance);
                SaveChemotherapySchemaInstanceAction(dataIn.ChemotherapySchemaInstanceId, userCookieData.Id, ChemotherapySchemaInstanceActionType.AddMedication, $"New medication ({medicationInstanceDb.Medication.Name}) is created and added to the schema instance.");
                SaveMedicationReplacements(medicationInstance, dataIn.MedicationIdsToReplace, userCookieData.Id);
            }

            return new ChemotherapySchemaResourceCreatedDTO() { 
                Id = medicationInstanceDb.MedicationInstanceId.ToString(), 
                RowVersion = Convert.ToBase64String(medicationInstanceDb.RowVersion),
                ParentId = medicationInstanceDb.MedicationId
            };
        }

        public AutocompleteResultDataOut GetMedicationInstanceDataForAutocomplete(MedicationInstanceAutocompleteDataIn dataIn)
        {
            var filtered = medicationInstanceDAL.FilterByNameAndChemotherapySchemaInstanceAndType(dataIn.Term, dataIn.ChemotherapySchemaInstanceId, dataIn.IsSupportiveMedication);

            var medicationInstanceDataOuts = filtered
                .OrderBy(x => x.Medication.Name).Skip(dataIn.Page * 15).Take(15)
                .Select(e => new AutocompleteDataOut()
                {
                    id = e.MedicationInstanceId.ToString(),
                    text = e.Medication.Name
                })
                .Where(x => string.IsNullOrEmpty(dataIn.ExcludeId) || !x.id.Equals(dataIn.ExcludeId))
                .ToList()
                ;

            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                pagination = new AutocompletePaginatioDataOut()
                {
                    more = Math.Ceiling(filtered.Count() / 15.00) > dataIn.Page,
                },
                results = medicationInstanceDataOuts
            };

            return result;
        }

        public SchemaTableDataOut GetSchemaTableData(int chemotherapySchemaInstanceId)
        {
            ChemotherapySchemaInstance chemotherapySchemaInstance = chemotherapySchemaInstanceDAL.GetSchemaInstance(chemotherapySchemaInstanceId);
            return GetSchemaTableData(chemotherapySchemaInstance);
        }

        public MedicationReplacementHistoryDataOut GetReplacementHistoryForMedication(int medicationId)
        {
            return new MedicationReplacementHistoryDataOut()
            {
                MedicationInstance = Mapper.Map<MedicationInstancePreviewDataOut>(medicationInstanceDAL.GetById(medicationId)),
                MedicationReplacements = Mapper.Map<List<MedicationReplacementDataOut>>(medicationReplacementDAL.GetByMedication(medicationId).ToList())
            };
        }

        private void SaveMedicationReplacements(MedicationInstance medicationInstance, List<int> medicationsToReplace, int creatorId)
        {
            if (medicationsToReplace.Count > 0)
            {
                var medicationsReplacements = medicationsToReplace.Select(id => new MedicationReplacement()
                {
                    ReplaceMedicationId = id,
                    ReplaceWithMedicationId = medicationInstance.MedicationInstanceId,
                    ChemotherapySchemaInstanceId = medicationInstance.ChemotherapySchemaInstanceId,
                    CreatorId = creatorId
                }).ToList();
                medicationReplacementDAL.InsertInBatch(medicationsReplacements);
                SaveChemotherapySchemaInstanceAction(medicationInstance.ChemotherapySchemaInstanceId, creatorId, ChemotherapySchemaInstanceActionType.ReplaceMedication, $"New medication ({medicationInstance.Medication.Name}) is set as an alternative for ({string.Join(",", medicationInstanceDAL.GetNameByIds(medicationsToReplace))}).");
            }
        }

        private SchemaTableDataOut GetSchemaTableData(ChemotherapySchemaInstance chemotherapySchemaInstance)
        {
            SchemaTableDataOut schemaTableData = new SchemaTableDataOut
            {
                Id = chemotherapySchemaInstance.ChemotherapySchemaInstanceId,
                RowVersion = Convert.ToBase64String(chemotherapySchemaInstance.RowVersion),
                FirstDay = chemotherapySchemaInstance.StartDate ?? DateTime.Now,
                Medications = Mapper.Map<List<SchemaTableMedicationInstanceDataOut>>(chemotherapySchemaInstance.Medications.Where(x => !x.IsDeleted)),
                History = Mapper.Map<List<ChemotherapySchemaInstanceVersionDataOut>>(chemotherapySchemaInstance.ChemotherapySchemaInstanceHistory),
                MedicationReplacements = Mapper.Map<List<MedicationReplacementDataOut>>(chemotherapySchemaInstance.MedicationReplacements)
            };
            schemaTableData.SetSchemaDays();

            return schemaTableData;
        }

        private void IsConcurrencyViolated(int schemaId, string rowVersion)
        {
            string rowVersionDb = Convert.ToBase64String(chemotherapySchemaInstanceDAL.GetRowVersion(schemaId));
            if (!rowVersion.Equals(rowVersionDb)) throw new DbUpdateConcurrencyException("Concurrency ex");
        }

        private void IsConcurrencyMedicationInstanceViolated(int medicationInstanceId, string rowVersion)
        {
            string rowVersionDb = Convert.ToBase64String(medicationInstanceDAL.GetRowVersion(medicationInstanceId));
            if (!rowVersion.Equals(rowVersionDb)) throw new DbUpdateConcurrencyException("Concurrency ex");
        }

        private MedicationDoseInstanceDataOut GetMedicationDoseInstanceDataOut(MedicationDoseInstance medicationDose, string rowVersion)
        {
            MedicationDoseInstanceDataOut dataOut = Mapper.Map<MedicationDoseInstanceDataOut>(medicationDose);
            dataOut.RowVersion = rowVersion;
            dataOut.StartsAt = medicationDose.GetStartTime();

            return dataOut;
        }

        private void SaveChemotherapySchemaInstanceAction(int chemotherapySchemaInstanceId, int creatorId, ChemotherapySchemaInstanceActionType actionType, string description, ChemotherapySchemaInstanceVersion chemotherapySchemaInstanceVersion = null)
        {
            chemotherapySchemaInstanceVersion = chemotherapySchemaInstanceVersion ?? new ChemotherapySchemaInstanceVersion();
            chemotherapySchemaInstanceVersion.ChemotherapySchemaInstanceId = chemotherapySchemaInstanceId;
            chemotherapySchemaInstanceVersion.CreatorId = creatorId;
            chemotherapySchemaInstanceVersion.ActionType = actionType;
            chemotherapySchemaInstanceVersion.Description = description;

            chemotherapySchemaInstanceHistoryDAL.InsertOrUpdate(chemotherapySchemaInstanceVersion);
        }
    }
}
