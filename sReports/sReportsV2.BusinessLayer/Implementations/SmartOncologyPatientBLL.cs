using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Extensions;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Sql.Entities.SmartOncologyPatient;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.SmartOncologyPatient.DataIn;
using sReportsV2.DTOs.DTOs.SmartOncologyPatient.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.SqlDomain.Filter;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class SmartOncologyPatientBLL : ISmartOncologyPatientBLL
    {
        private readonly ISmartOncologyPatientDAL smartOncologyPatientDAL;
        private readonly IUserDAL userDAL;

        public SmartOncologyPatientBLL(ISmartOncologyPatientDAL smartOncologyPatientDAL, IUserDAL userDAL)
        {
            this.smartOncologyPatientDAL = smartOncologyPatientDAL;
            this.userDAL = userDAL;
        }
        public void Delete(int patientId)
        {
            smartOncologyPatientDAL.Delete(patientId);
        }

        public PaginationDataOut<SmartOncologyPatientPreviewDataOut, SmartOncologyPatientFilterDataIn> GetAllFiltered(SmartOncologyPatientFilterDataIn dataIn)
        {
            SmartOncologyPatientFilter filter = Mapper.Map<SmartOncologyPatientFilter>(dataIn);
            PaginationDataOut<SmartOncologyPatientPreviewDataOut, SmartOncologyPatientFilterDataIn> result = new PaginationDataOut<SmartOncologyPatientPreviewDataOut, SmartOncologyPatientFilterDataIn>()
            {
                Count = (int)this.smartOncologyPatientDAL.GetAllEntriesCount(filter),
                Data = Mapper.Map<List<SmartOncologyPatientPreviewDataOut>>(this.smartOncologyPatientDAL.GetAll(filter)),
                DataIn = dataIn
            };

            return result;
        }

        public AutocompleteResultDataOut GetAutocompletePatientData(AutocompleteDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            var filtered = smartOncologyPatientDAL.GetPatientsByName(dataIn.Term);
            var enumDataOuts = filtered
                .OrderBy(x => x.Name.Given).Skip(dataIn.Page * 15).Take(15)
                .Select(e => new AutocompleteDataOut()
                {
                    id = e.SmartOncologyPatientId.ToString(),
                    text = e.Name.Given + " " + e.Name.Family
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

        public SmartOncologyPatientDataOut GetById(int id)
        {
            SmartOncologyPatient patient = smartOncologyPatientDAL.GetById(id);
            if (patient == null) throw new NullReferenceException();

            SmartOncologyPatientDataOut patientDataOut = Mapper.Map<SmartOncologyPatientDataOut>(patient);
            patientDataOut.ClinicalTrials = Mapper.Map<List<ClinicalTrialDTO>>(userDAL.GetlClinicalTrialByIds(patient.GetClinicalTrialIds()));
            return patientDataOut;
        }

        public SmartOncologyPatientPreviewDataOut GetPreviewById(int id)
        {
            SmartOncologyPatient patient = smartOncologyPatientDAL.GetById(id);
            if (patient == null) throw new NullReferenceException();

            SmartOncologyPatientPreviewDataOut patientDataOut = Mapper.Map<SmartOncologyPatientPreviewDataOut>(patient);
            return patientDataOut;
        }

        public ResourceCreatedDTO InsertOrUpdate(SmartOncologyPatientDataIn patientDataIn, UserCookieData userCookieData)
        {
            SmartOncologyPatient patient = Mapper.Map<SmartOncologyPatient>(patientDataIn);
            SmartOncologyPatient patientDb = smartOncologyPatientDAL.GetById(patient.SmartOncologyPatientId);
            if (patientDb == null)
            {
                patientDb = patient;
            }
            else
            {
                patientDb.Copy(patient);
            }
            patientDb.PatientInformedBy = string.Concat(userCookieData.FirstName, " ", userCookieData.LastName);
            smartOncologyPatientDAL.InsertOrUpdate(patientDb);

            return new ResourceCreatedDTO()
            {
                Id = patientDb.SmartOncologyPatientId.ToString()
            };
        }
    }
}
