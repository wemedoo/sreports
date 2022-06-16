using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Patient;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class PatientBLL : IPatientBLL
    {
        private readonly IPatientDAL patientDAL;

        public PatientBLL(IPatientDAL patientDAL)
        {
            this.patientDAL = patientDAL;
        }

        public ResourceCreatedDTO Insert(PatientDataIn patientDataIn)
        {
            Patient patient = Mapper.Map<Patient>(patientDataIn);
            Patient patientDb = patientDAL.GetById(patient.Id);
            if(patientDb == null)
            {
                patientDb = patient;
            }
            else
            {
                patientDb.Copy(patient);
            }
            patientDAL.InsertOrUpdate(patientDb);

            return new ResourceCreatedDTO()
            {
                Id = patientDb.Id,
                RowVersion = Convert.ToBase64String(patientDb.RowVersion),
                LastUpdate = patientDb.LastUpdate
            };
        }

        private void SetIdentifiers(List<Identifier> dbIdentifiers, List<Identifier> identifiers, int patientId)
        {
            foreach (var identifier in identifiers.Where(x => x.Id == 0).ToList())
            {
                dbIdentifiers.Add(identifier);
            }

        }
    }
}
