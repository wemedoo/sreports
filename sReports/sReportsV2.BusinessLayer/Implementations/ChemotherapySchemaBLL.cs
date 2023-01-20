using AutoMapper;
using ExcelImporter.Importers;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Exceptions;
using sReportsV2.Common.Extensions;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataIn;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataOut;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DTO;
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
    public class ChemotherapySchemaBLL : IChemotherapySchemaBLL
    {
        private readonly IChemotherapySchemaDAL chemotherapySchemaDAL;
        private readonly ILiteratureReferenceDAL literatureReferenceDAL;
        private readonly IMedicationDAL medicationDAL;
        private readonly IMedicationDoseDAL medicationDoseDAL;
        private readonly IBodySurfaceCalculationFormulaDAL bodySurfaceCalculationFormulaDAL;
        private readonly IRouteOfAdministrationDAL routeOfAdministrationDAL;
        private readonly IMedicationDoseTypeDAL medicationDoseTypeDAL;
        private readonly IUserDAL userDAL;
        private readonly IUnitDAL unitDAL;

        public ChemotherapySchemaBLL(IChemotherapySchemaDAL chemotherapySchemaDAL, ILiteratureReferenceDAL literatureReferenceDAL, IMedicationDAL medicationDAL, IMedicationDoseDAL medicationDoseDAL, IBodySurfaceCalculationFormulaDAL bodySurfaceCalculationFormulaDAL, IRouteOfAdministrationDAL routeOfAdministrationDAL, IMedicationDoseTypeDAL medicationDoseTypeDAL, IUserDAL userDAL, IUnitDAL unitDAL)
        {
            this.chemotherapySchemaDAL = chemotherapySchemaDAL;
            this.literatureReferenceDAL = literatureReferenceDAL;
            this.medicationDAL = medicationDAL;
            this.medicationDoseDAL = medicationDoseDAL;
            this.bodySurfaceCalculationFormulaDAL = bodySurfaceCalculationFormulaDAL;
            this.routeOfAdministrationDAL = routeOfAdministrationDAL;
            this.medicationDoseTypeDAL = medicationDoseTypeDAL;
            this.userDAL = userDAL;
            this.unitDAL = unitDAL;
        }

        public ChemotherapySchemaDataOut GetById(int id)
        {
            ChemotherapySchema chemotherapySchema = chemotherapySchemaDAL.GetById(id);
            if (chemotherapySchema == null) throw new NullReferenceException();

            ChemotherapySchemaDataOut chemotherapySchemaDataOut = Mapper.Map<ChemotherapySchemaDataOut>(chemotherapySchema);
            SetRouteOfAdministrationForMedications(chemotherapySchemaDataOut);
            return chemotherapySchemaDataOut;
        }

        public ResourceCreatedDTO InsertOrUpdate(ChemotherapySchemaDataIn dataIn, UserCookieData userCookieData)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            ChemotherapySchema chemotherapySchema = Mapper.Map<ChemotherapySchema>(dataIn);
            ChemotherapySchema chemotherapySchemaDb = chemotherapySchemaDAL.GetById(dataIn.Id);

            if (chemotherapySchemaDb == null)
            {
                chemotherapySchemaDb = chemotherapySchema;
                SetChemotherapySchemaCreator(chemotherapySchemaDb, userCookieData);
            }
            else
            {
                chemotherapySchemaDb.Copy(chemotherapySchema);
            }
            InsertOrUpdate(chemotherapySchemaDb);

            return new ResourceCreatedDTO() { 
                Id = chemotherapySchemaDb.ChemotherapySchemaId.ToString(),
                RowVersion = Convert.ToBase64String(chemotherapySchemaDb.RowVersion)
            };
        }

        public ResourceCreatedDTO UpdateGeneralProperties(EditGeneralPropertiesDataIn dataIn, UserCookieData userCookieData)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            ChemotherapySchema chemotherapySchema = Mapper.Map<ChemotherapySchema>(dataIn);
            ChemotherapySchema chemotherapySchemaDb = chemotherapySchemaDAL.GetById(dataIn.Id);
            if (chemotherapySchemaDb == null)
            {
                chemotherapySchemaDb = chemotherapySchema;
                SetChemotherapySchemaCreator(chemotherapySchemaDb, userCookieData);
            }
            {
                chemotherapySchemaDb.CopyRowVersion(chemotherapySchema.RowVersion);
                chemotherapySchemaDb.CopyGeneralProperties(chemotherapySchema);
            }
            InsertOrUpdate(chemotherapySchemaDb);

            return new ResourceCreatedDTO() { 
                Id = chemotherapySchemaDb.ChemotherapySchemaId.ToString(),
                RowVersion = Convert.ToBase64String(chemotherapySchemaDb.RowVersion)
            };
        }

        public ResourceCreatedDTO UpdateName(EditNameDataIn nameDataIn, UserCookieData userCookieData)
        {
            nameDataIn = Ensure.IsNotNull(nameDataIn, nameof(nameDataIn));
            ChemotherapySchema chemotherapySchema = Mapper.Map<ChemotherapySchema>(nameDataIn);
            ChemotherapySchema chemotherapySchemaDb = chemotherapySchemaDAL.GetById(nameDataIn.Id);
            if (chemotherapySchemaDb == null)
            {
                chemotherapySchemaDb = chemotherapySchema;
                SetChemotherapySchemaCreator(chemotherapySchemaDb, userCookieData);
            }
            else
            {
                chemotherapySchemaDb.CopyRowVersion(chemotherapySchema.RowVersion);
                chemotherapySchemaDb.CopyName(chemotherapySchema.Name);
            }
            InsertOrUpdate(chemotherapySchemaDb);

            return new ResourceCreatedDTO() { 
                Id = chemotherapySchemaDb.ChemotherapySchemaId.ToString(),
                RowVersion = Convert.ToBase64String(chemotherapySchemaDb.RowVersion)
            };
        }

        public ChemotherapySchemaDataOut UpdateIndications(EditIndicationsDataIn indicationsDataIn, UserCookieData userCookieData)
        {
            ChemotherapySchema chemotherapySchema = Mapper.Map<ChemotherapySchema>(indicationsDataIn);
            ChemotherapySchema chemotherapySchemaDb = chemotherapySchemaDAL.GetById(indicationsDataIn.ChemotherapySchemaId);
            if (chemotherapySchemaDb == null)
            {
                chemotherapySchemaDb = chemotherapySchema;
                SetChemotherapySchemaCreator(chemotherapySchemaDb, userCookieData);
            }
            else
            {
                chemotherapySchemaDb.CopyRowVersion(chemotherapySchema.RowVersion);
                chemotherapySchemaDb.CopyIndications(chemotherapySchema.Indications);
            }
            InsertOrUpdate(chemotherapySchemaDb);

            return Mapper.Map<ChemotherapySchemaDataOut>(chemotherapySchemaDb);
        }

        public ChemotherapySchemaResourceCreatedDTO UpdateReference(EditLiteratureReferenceDataIn dataIn, UserCookieData userCookieData)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            IsUniquePMID(dataIn.LiteratureReference.PubMedID, dataIn.LiteratureReference.Id);

            LiteratureReference literatureReference = Mapper.Map<LiteratureReference>(dataIn.LiteratureReference);
            LiteratureReference literatureReferenceDb = literatureReferenceDAL.GetById(literatureReference.LiteratureReferenceId);

            if (literatureReferenceDb == null)
            {
                literatureReferenceDb = literatureReference;
            }
            else
            {
                literatureReferenceDb.Copy(literatureReference);
            }

            int chemotherapySchemaId;
            int referenceId;
            string rowVersion;
            if (IsSchemaAlreadyCreated(literatureReferenceDb.ChemotherapySchemaId))
            {
                literatureReferenceDAL.InsertOrUpdate(literatureReferenceDb);
                referenceId = literatureReferenceDb.LiteratureReferenceId;
                chemotherapySchemaId = literatureReferenceDb.ChemotherapySchemaId;
                rowVersion = dataIn.RowVersion;
                IsConcurrencyViolated(chemotherapySchemaId, rowVersion);
            }
            else
            {
                ChemotherapySchema newChemotherapySchema = new ChemotherapySchema() { LiteratureReferences = new List<LiteratureReference>() { literatureReferenceDb } };
                SetChemotherapySchemaCreator(newChemotherapySchema, userCookieData);
                InsertOrUpdate(newChemotherapySchema);

                chemotherapySchemaId = newChemotherapySchema.ChemotherapySchemaId;
                referenceId = newChemotherapySchema.LiteratureReferences.First().LiteratureReferenceId;
                rowVersion = Convert.ToBase64String(newChemotherapySchema.RowVersion);
            }

            return new ChemotherapySchemaResourceCreatedDTO() { ParentId = chemotherapySchemaId, Id = referenceId.ToString(), RowVersion = rowVersion};
        }

        public MedicationDataOut GetMedication(int id)
        {
            Medication medication = medicationDAL.GetById(id);

            MedicationDataOut medicationDataOut = Mapper.Map<MedicationDataOut>(medication);
            return medicationDataOut;
        }

        public ResourceCreatedDTO UpdateMedication(MedicationDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            Medication medication = Mapper.Map<Medication>(dataIn);
            Medication medicationDb = medicationDAL.GetById(dataIn.Id);

            if (medicationDb == null)
            {
                medicationDb = medication;
            }
            else
            {
                medicationDb.Copy(medication);
            }

            InsertOrUpdateMedication(medicationDb);

            return new ResourceCreatedDTO() { Id = medicationDb.MedicationId.ToString(), RowVersion = Convert.ToBase64String(medicationDb.RowVersion) };
        }

        public ResourceCreatedDTO UpdateMedicationDose(MedicationDoseDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            MedicationDose medicationDose = Mapper.Map<MedicationDose>(dataIn);
            MedicationDose medicationDoseDb = medicationDoseDAL.GetById(dataIn.Id);

            if (medicationDoseDb == null)
            {
                medicationDoseDb = medicationDose;
            }
            else
            {
                medicationDoseDb.Copy(medicationDose);
            }

            IsConcurrencyMedicationViolated(dataIn.MedicationId, dataIn.RowVersion);
            medicationDoseDAL.InsertOrUpdate(medicationDoseDb);

            return new ResourceCreatedDTO() { Id = medicationDoseDb.MedicationDoseId.ToString(), RowVersion = dataIn.RowVersion };
        }

        public EditMedicationDoseInBatchDataOut UpdateMedicationDoseInBatch(EditMedicationDoseInBatchDataIn dataIn)
        {
            Medication medication = Mapper.Map<Medication>(dataIn);
            Medication medicationDb = medicationDAL.GetById(medication.MedicationId);

            List<MedicationDose> upcomingMedicationDoses = medicationDb.GetPremedicationsDays();
            List<MedicationDose> medicationDayDoses = medication.MedicationDoses;
            upcomingMedicationDoses.AddRange(medicationDayDoses);

            medicationDb.UpdateMedicationDoses(upcomingMedicationDoses);
            medicationDb.CopyRowVersion(medication.RowVersion);

            InsertOrUpdateMedication(medicationDb);

            Dictionary<string, int> idsPerDays = medicationDb.GetMedicationDays().ToDictionary(x => x.DayNumber.ToString(), x => x.MedicationDoseId);
            return new EditMedicationDoseInBatchDataOut { IdsPerDays = idsPerDays, RowVersion = Convert.ToBase64String(medicationDb.RowVersion) };
        }

        public void DeleteDose(EditMedicationDoseDataIn dataIn)
        {
            IsConcurrencyMedicationViolated(dataIn.MedicationId, dataIn.RowVersion);
            medicationDoseDAL.Delete(dataIn.Id);
        }

        public LiteratureReferenceDataOut GetReference(int id)
        {
            LiteratureReference literatureReference = literatureReferenceDAL.GetById(id);
            return Mapper.Map<LiteratureReferenceDataOut>(literatureReference);
        }

        public List<BodySurfaceCalculationFormulaDTO> GetFormulas()
        {
            return Mapper.Map<List<BodySurfaceCalculationFormulaDTO>>(bodySurfaceCalculationFormulaDAL.GetAll());
        }

        public AutocompleteResultDataOut GetRouteOfAdministrationDataForAutocomplete(AutocompleteDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            List<AutocompleteDataOut> routeOfAdministrationDataOuts = new List<AutocompleteDataOut>();
            IQueryable<RouteOfAdministration> filtered = routeOfAdministrationDAL.FilterByName(dataIn.Term);
            routeOfAdministrationDataOuts = filtered.OrderBy(x => x.Name).Skip(dataIn.Page * 15).Take(15)
                .Select(x => new AutocompleteDataOut()
                {
                    id = x.RouteOfAdministrationId.ToString(),
                    text = x.Name
                })
                .Where(x => string.IsNullOrEmpty(dataIn.ExcludeId) || !x.id.Equals(dataIn.ExcludeId))
                .ToList();

            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                pagination = new AutocompletePaginatioDataOut()
                {
                    more = Math.Ceiling(filtered.Count() / 15.00) > dataIn.Page,
                },
                results = routeOfAdministrationDataOuts
            };

            return result;
        }

        public RouteOfAdministrationDTO GetRouteOfAdministration(int id)
        {
            return Mapper.Map<RouteOfAdministrationDTO>(routeOfAdministrationDAL.GetById(id));
        }

        public AutocompleteResultDataOut GetUnitDataForAutocomplete(AutocompleteDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            List<AutocompleteDataOut> routeOfAdministrationDataOuts = new List<AutocompleteDataOut>();
            IQueryable<Unit> filtered = unitDAL.FilterByName(dataIn.Term);
            routeOfAdministrationDataOuts = filtered.OrderBy(x => x.Name).Skip(dataIn.Page * 15).Take(15)
                .Select(x => new AutocompleteDataOut()
                {
                    id = x.UnitId.ToString(),
                    text = x.Name
                })
                .Where(x => string.IsNullOrEmpty(dataIn.ExcludeId) || !x.id.Equals(dataIn.ExcludeId))
                .ToList();

            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                pagination = new AutocompletePaginatioDataOut()
                {
                    more = Math.Ceiling(filtered.Count() / 15.00) > dataIn.Page,
                },
                results = routeOfAdministrationDataOuts
            };

            return result;
        }

        public UnitDTO GetUnit(int id)
        {
            return Mapper.Map<UnitDTO>(unitDAL.GetById(id));
        }

        public List<MedicationPreviewDoseTypeDTO> GetMedicationDoseTypes()
        {
            return Mapper.Map<List<MedicationPreviewDoseTypeDTO>>(medicationDoseTypeDAL.GetAll());
        }

        public PaginationDataOut<ChemotherapySchemaDataOut, DataIn> ReloadTable(ChemotherapySchemaFilterDataIn dataIn)
        {
            Ensure.IsNotNull(dataIn, nameof(dataIn));

            ChemotherapySchemaFilter filterData = Mapper.Map<ChemotherapySchemaFilter>(dataIn);
            PaginationDataOut<ChemotherapySchemaDataOut, DataIn> result = new PaginationDataOut<ChemotherapySchemaDataOut, DataIn>()
            {
                Count = (int)chemotherapySchemaDAL.GetAllFilteredCount(filterData),
                Data = Mapper.Map<List<ChemotherapySchemaDataOut>>(chemotherapySchemaDAL.GetAll(filterData)),
                DataIn = dataIn
            };

            return result;
        }

        public MedicationDoseTypeDTO GetMedicationDoseType(int id)
        {
            return Mapper.Map<MedicationDoseTypeDTO>(medicationDoseTypeDAL.GetById(id));
        }

        public AutocompleteResultDataOut GetDataForAutocomplete(AutocompleteDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            var filtered = chemotherapySchemaDAL.FilterByName(dataIn.Term);
            var enumDataOuts = filtered
                .OrderBy(x => x.Name).Skip(dataIn.Page * 15).Take(15)
                .Select(e => new AutocompleteDataOut()
                {
                    id = e.ChemotherapySchemaId.ToString(),
                    text = e.Name
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
                results = enumDataOuts
            };

            return result;
        }

        public void Delete(int id)
        {
            chemotherapySchemaDAL.Delete(id);
        }

        public SchemaTableDataOut GetSchemaDefinition(int id, DateTime? firstDay)
        {
            ChemotherapySchema chemotherapySchema = chemotherapySchemaDAL.GetSchemaDefinition(id);

            if(chemotherapySchema != null)
            {
                return GetSchemaTableData(chemotherapySchema, firstDay);
            }
            else
            {
                return null;
            }
        }
        public void ParseExcelDataAndInsert(int creatorId)
        {
            SchemaImporterV2 schemaImporterV2 = new SchemaImporterV2("Chemotherapy Compendium Import - 26.11.2021", "Basic Data", chemotherapySchemaDAL, routeOfAdministrationDAL, unitDAL, creatorId);
            schemaImporterV2.ImportDataFromExcelToDatabase();
        }

        private bool IsSchemaAlreadyCreated(int schemaId)
        {
            return schemaId > 0;
        }

        private void SetRouteOfAdministrationForMedications(ChemotherapySchemaDataOut chemotherapySchema)
        {
            foreach(MedicationPreviewDataOut medication in chemotherapySchema.Medications)
            {
                if(int.TryParse(medication.RouteOfAdministration, out int routeOfAdministrationId))
                {
                    RouteOfAdministration routeOfAdministration = routeOfAdministrationDAL.GetById(routeOfAdministrationId);
                    medication.RouteOfAdministrationDTO = Mapper.Map<RouteOfAdministrationDTO>(routeOfAdministration);
                }
            }
        }

        private void SetChemotherapySchemaCreator(ChemotherapySchema chemotherapySchema, UserCookieData userCookieData)
        {
            chemotherapySchema.Creator = userDAL.GetById(userCookieData.Id);
        }

        private SchemaTableDataOut GetSchemaTableData(ChemotherapySchema chemotherapySchema, DateTime? firstDay)
        {
            SchemaTableDataOut schemaTableData = new SchemaTableDataOut
            {
                FirstDay = firstDay ?? DateTime.Now,
                Medications = chemotherapySchema.Medications.Select(i => new SchemaTableMedicationInstanceDataOut() {
                    Medication = Mapper.Map<SchemaTableMedicationDataOut>(i)
                }).ToList()
            };
            schemaTableData.SetSchemaDays(true);

            return schemaTableData;
        }

        private void IsConcurrencyViolated(int schemaId, string rowVersion)
        {
            string rowVersionDb = Convert.ToBase64String(chemotherapySchemaDAL.GetRowVersion(schemaId));
            if (!rowVersion.Equals(rowVersionDb)) throw new DbUpdateConcurrencyException("Concurrency ex");
        }

        private void IsConcurrencyMedicationViolated(int medicationId, string rowVersion)
        {
            string rowVersionDb = Convert.ToBase64String(medicationDAL.GetRowVersion(medicationId));
            if (!rowVersion.Equals(rowVersionDb)) throw new DbUpdateConcurrencyException("Concurrency ex");
        }

        private void InsertOrUpdate(ChemotherapySchema chemotherapySchema)
        {
            try
            {
                chemotherapySchemaDAL.InsertOrUpdate(chemotherapySchema);
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw e;
            }
        }

        private void InsertOrUpdateMedication(Medication medication)
        {
            try
            {
                medicationDAL.InsertOrUpdate(medication);
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw e;
            }
        }

        private void IsUniquePMID(int pmid, int referenceId)
        {
            int? referenceIdWithGivenPMID = literatureReferenceDAL.FindByPMID(pmid);
            if(referenceIdWithGivenPMID.HasValue && referenceIdWithGivenPMID.Value != referenceId)
            {
                throw new DuplicateException($"Literature reference with given PMID ({pmid}) is already defined!");
            }
        }
    }
}
