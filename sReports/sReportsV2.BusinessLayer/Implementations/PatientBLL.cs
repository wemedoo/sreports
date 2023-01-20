using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Entities.User;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.Patient;
using sReportsV2.DTOs.Patient.DataIn;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Web.Configuration;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class PatientBLL : IPatientBLL
    {
        private readonly IPatientDAL patientDAL;
        private readonly IThesaurusDAL thesaurusDAL;

        public PatientBLL(IPatientDAL patientDAL, IThesaurusDAL thesaurusDAL)
        {
            this.patientDAL = patientDAL;
            this.thesaurusDAL = thesaurusDAL;
        }

        public void Delete(int patientId)
        {
            patientDAL.Delete(patientId);
        }

        public bool ExistsPatientByIdentifier(Identifier identifier)
        {
            return patientDAL.ExistsPatientByIdentifier(identifier);
        }

        public PaginationDataOut<PatientDataOut, DataIn> GetAllFiltered(PatientFilterDataIn dataIn)
        {
            PatientFilter filter = Mapper.Map<PatientFilter>(dataIn);
            PaginationDataOut<PatientDataOut, DataIn> result = new PaginationDataOut<PatientDataOut, DataIn>()
            {
                Count = (int)this.patientDAL.GetAllEntriesCount(filter),
                Data = Mapper.Map<List<PatientDataOut>>(this.patientDAL.GetAll(filter)),
                DataIn = dataIn
            };

            return result;
        }

        public PatientDataOut GetById(int id)
        {
            Patient patient = patientDAL.GetById(id);
            if (patient == null) throw new NullReferenceException("Patient does not exist");

            return Mapper.Map<PatientDataOut>(patient);
        }

        public PatientDataOut GetByIdentifier(Identifier identifier)
        {
            Patient patient = patientDAL.GetByIdentifier(identifier);
            if (patient == null) throw new NullReferenceException();

            return Mapper.Map<PatientDataOut>(patient);
        }

        public ResourceCreatedDTO InsertOrUpdate(PatientDataIn patientDataIn, UserData userData)
        {
            Patient patient = Mapper.Map<Patient>(patientDataIn);
            patient.OrganizationId = userData.ActiveOrganization.GetValueOrDefault();
            Patient patientDb = patientDAL.GetById(patient.PatientId);
            int defaultIdentifierId = 0;

            if (patientDb == null)
            {
                patientDb = patient;
                defaultIdentifierId = thesaurusDAL.GetIdByPreferredTerm(WebConfigurationManager.AppSettings["DefaultIdentifier"]);
            }
            else
            {
                patientDb.Copy(patient);
            }
            patientDAL.InsertOrUpdate(patientDb, defaultIdentifierId);

            return new ResourceCreatedDTO()
            {
                Id = patientDb.PatientId.ToString(),
                RowVersion = Convert.ToBase64String(patientDb.RowVersion)
            };
        }

        public List<PatientDataOut> GetPatientsByName(string searchValue)
        {
            
            List<AutoCompletePatientData> patients = patientDAL.GetPatientsFilteredByName(searchValue);

            List<PatientDataOut> result = new List<PatientDataOut>();

            foreach (AutoCompletePatientData patient in patients)
            {
                result.Add(new PatientDataOut { Id = patient.PatientId, Name = patient.Name, FamilyName = patient.FamilyName });
            }

            return result;
        }

        public List<PatientDataOut> GetPatientsByFirstAndLastName(PatientSearchFilter patientSearchFilter)
        {
            List<PatientDataOut> result = new List<PatientDataOut>();
            List<AutoCompletePatientData> patients = patientDAL.GetPatientsFilteredByFirstAndLastName(patientSearchFilter);

            foreach (AutoCompletePatientData patient in patients)
            {
                result.Add(new PatientDataOut { Id = patient.PatientId, Name = patient.Name, FamilyName = patient.FamilyName, BirthDate = patient.BirthDate });
            }

            return result;
        }
    }
}
