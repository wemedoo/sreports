using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using sReportsV2.Common.Constants;

namespace sReportsV2.SqlDomain.Implementations
{
    public class PatientDAL : IPatientDAL
    {
        private SReportsContext context;
        public PatientDAL(SReportsContext context)
        {
            this.context = context;
        }

        public bool Delete(int patientId, DateTime lastUpdate)
        {
            this.GetById(patientId).IsDeleted = true;
            context.SaveChanges();
            
            return true;
        }

        public bool ExistsPatientByIdentifier(Identifier identifier)
        {
            if (identifier.System.Equals(ResourceTypes.O4PatientId))
            {
                return ExistsPatietById(identifier.Value);
            }

            return context.Patients
                .Include(x => x.Identifiers)
                .Include(x => x.Name)
                .Include(x => x.MultipleB)
                .Include(x => x.Telecoms)
                .Include(x => x.Communications)
                .Include(x => x.EpisodeOfCares)
                .Where(x => !x.IsDeleted && x.Identifiers
                    .Any(y => y.Value.Equals(identifier.Value) && y.System.Equals(identifier.System)))
                .Count() > 0;
        }
        public bool ExistsPatietById(string umcn)
        {
            return this.context.Patients
                .Include(x => x.Identifiers)
                .Include(x => x.Name)
                .Include(x => x.MultipleB)
                .Include(x => x.Telecoms)
                .Include(x => x.Communications)
                .Include(x => x.EpisodeOfCares)
                .Any(x => !x.IsDeleted && x.Identifiers.Any(y => y.Value.Equals(umcn)));
        }

        public bool ExistsPatient(int id)
        {
            return this.context.Patients
                .Include(x => x.Identifiers)
                .Include(x => x.Name)
                .Include(x => x.MultipleB)
                .Include(x => x.Telecoms)
                .Include(x => x.Communications)
                .Include(x => x.EpisodeOfCares).Any(x => !x.IsDeleted && x.Id.Equals(id));
        }


        public List<Patient> GetAll(PatientFilter filter)
        {
           var result = this.GetPatientFiltered(filter)
                .OrderByDescending(x => x.Id)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            return result;
        }

        public int GetAllEntriesCount(PatientFilter filter)
        {
            return this.GetPatientFiltered(filter).Count();
        }

        public Patient GetById(int id)
        {
            return context.Patients
                .Include(x => x.Addresss)
                .Include(x => x.Identifiers)
                .Include(x => x.MultipleB)
                .Include(x => x.Telecoms)
                .Include(x => x.Communications)
                .Include(x => x.EpisodeOfCares)
                .Include(x => x.ContactPerson)
                .Include(x => x.ContactPerson.Address)
                .Include(x => x.ContactPerson.Telecoms)
                .FirstOrDefault(x => x.Id == id);
        }

        public Patient GetByIdentifier(Identifier identifier)
        {
            Patient result = null;
            if (identifier.System.Equals(ResourceTypes.O4PatientId))
            {
                result = GetById(Int32.Parse(identifier.Value));
            }
            else
            {
                result = context.Patients
                    .Include(x => x.Identifiers)
                    .Include(x => x.Name)
                    .Include(x => x.MultipleB)
                    .Include(x => x.Telecoms)
                    .Include(x => x.Communications)
                    .Where(x => !x.IsDeleted && x.Identifiers
                        .Any(y => y.Value.Equals(identifier.Value) && y.System.Equals(identifier.System)))
                    .FirstOrDefault();
            }
            return result;
        }

        public void InsertOrUpdate(Patient patient)
        {
            
            if (patient.Id == 0)
            {
                patient.EntryDatetime = DateTime.Now;
                patient.LastUpdate = DateTime.Now;
                context.Patients.Add(patient);
            }
            else 
            {
                patient.LastUpdate = DateTime.Now;        
            }

            context.SaveChanges();
        }

        private IQueryable<Patient> GetPatientFiltered(PatientFilter filter)
        {
            IQueryable<Patient> patientQuery = this.context.Patients
                .Include(x => x.Identifiers)
                .Include(x => x.MultipleB)
                .Include(x => x.Telecoms)
                .Include(x => x.Communications)
                .Include(x => x.ContactPerson)
                .Where(x => !x.IsDeleted);

            if (filter.BirthDate != null)
            {
                patientQuery = patientQuery.Where(x => x.BirthDate == filter.BirthDate);
            }

            if (!string.IsNullOrEmpty(filter.City))
            {
                patientQuery = patientQuery.Where(x => x.Addresss != null && x.Addresss.City != null && x.Addresss.City.Contains(filter.City));
            }

            if (!string.IsNullOrEmpty(filter.Country))
            {
                patientQuery = patientQuery.Where(x => x.Addresss != null && x.Addresss.Country != null && x.Addresss.Country.Contains(filter.Country));
            }

            if (!string.IsNullOrEmpty(filter.Family))
            {
                patientQuery = patientQuery.Where(x => x.Name != null && x.Name.Family != null && x.Name.Family.Contains(filter.Family));
            }

            if (!string.IsNullOrEmpty(filter.Given))
            {
                patientQuery = patientQuery.Where(x => x.Name != null && x.Name.Given != null && x.Name.Given.Contains(filter.Given));
            }
            if (!string.IsNullOrEmpty(filter.PostalCode))
            {
                patientQuery = patientQuery.Where(x => x.Addresss != null && x.Addresss.PostalCode != null && x.Addresss.PostalCode.Contains(filter.PostalCode));
            }
            patientQuery = FilterByIdentifier(patientQuery, filter.IdentifierType, filter.IdentifierValue);

            return patientQuery;
        }

        private IQueryable<Patient> FilterByIdentifier(IQueryable<Patient> patientEntities, string system, string value)
        {
            IQueryable<Patient> result = null;
            if (!string.IsNullOrEmpty(system) && !string.IsNullOrEmpty(value))
            {
                if (system.Equals(ResourceTypes.O4PatientId))
                {
                    result = patientEntities.Where(x => x.Id.Equals(value));
                }
                else
                {
                    result = patientEntities.Where(x => x.Identifiers.Any(y => !string.IsNullOrEmpty(y.System) && !string.IsNullOrEmpty(y.Value) && y.System.Equals(system) && y.Value.Equals(value)));
                }
            }
            else
            {
                result = patientEntities;
            }

            return result;
        }
    }
}
