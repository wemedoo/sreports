using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers;
using sReportsV2.Common.Enums;

namespace sReportsV2.SqlDomain.Implementations
{
    public class PatientDAL : IPatientDAL
    {
        private SReportsContext context;
        public PatientDAL(SReportsContext context)
        {
            this.context = context;
        }

        public void Delete(int patientId)
        {
            Patient fromDb = this.GetById(patientId);
            if (fromDb != null)
            {
                fromDb.IsDeleted = true;
                fromDb.SetLastUpdate();
                context.SaveChanges();
            }
        }

        public bool ExistsPatientByIdentifier(Identifier identifier)
        {
            if (identifier.System.Equals(ResourceTypes.O4PatientId))
            {
                return ExistsPatietById(identifier.Value);
            }

            return context.Patients
                .Include(x => x.Identifiers)
                .Where(x => !x.IsDeleted && x.Identifiers
                    .Any(y => y.Value.Equals(identifier.Value) && y.System.Equals(identifier.System)))
                .Count() > 0;
        }
        public bool ExistsPatietById(string umcn)
        {
            return this.context.Patients
                .Include(x => x.Identifiers)
                .Any(x => !x.IsDeleted && x.Identifiers.Any(y => y.Value.Equals(umcn)));
        }

        public bool ExistsPatient(int id)
        {
            return this.context.Patients.Any(x => !x.IsDeleted && x.PatientId.Equals(id));
        }

        public List<Patient> GetAll(PatientFilter filter)
        {
            IQueryable<Patient> result = GetPatientFiltered(filter);

            if (filter.ColumnName != null)
                result = SortByField(result, filter);
            else
                result = result.OrderByDescending(x => x.PatientId)
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize);

            return result.ToList();
        }

        public int GetAllEntriesCount(PatientFilter filter)
        {
            return this.GetPatientFiltered(filter).Count();
        }

        public Patient GetById(int id)
        {
            return context.Patients
                .Include(x => x.Addresses)
                .Include(x => x.Identifiers)
                .Include(x => x.MultipleBirth)
                .Include(x => x.PatientTelecoms)
                .Include(x => x.Communications)
                .Include(x => x.EpisodeOfCares)
                .Include(x => x.Contact)
                .Include(x => x.Contact.Address)
                .Include(x => x.Contact.Telecoms)
                .FirstOrDefault(x => x.PatientId == id);
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
                    .Include(x => x.MultipleBirth)
                    .Include(x => x.PatientTelecoms)
                    .Include(x => x.Communications)
                    .Where(x => !x.IsDeleted && x.Identifiers
                        .Any(y => y.Value.Equals(identifier.Value) && y.System.Equals(identifier.System)))
                    .FirstOrDefault();
            }
            return result;
        }

        public void InsertOrUpdate(Patient patient, int defaultIdentifierId)
        {
            if (patient.PatientId == 0)
            {
                context.Patients.Add(patient);
                if (defaultIdentifierId != 0)
                {
                    context.SaveChanges();
                    patient.Identifiers.Add(new Identifier { System = defaultIdentifierId.ToString(), Value = patient.PatientId.ToString() });
                }
            }
            else
            {
                patient.SetLastUpdate();      
            }

            context.SaveChanges();
        }

        public List<Patient> GetAllByIds(List<int> ids)
        {
            return context.Patients.Where(x => ids.Contains(x.PatientId)).ToList();
        }

        private IQueryable<Patient> GetPatientFiltered(PatientFilter filter)
        {
            IQueryable<Patient> patientQuery = this.context.Patients
                .Include(x => x.Identifiers)
                .Include(x => x.MultipleBirth)
                .Include(x => x.PatientTelecoms)
                .Include(x => x.Communications)
                .Include(x => x.Contact)
                .Where(x => !x.IsDeleted && x.OrganizationId == filter.OrganizationId);

            if (filter.BirthDate != null)
            {
                patientQuery = patientQuery.Where(x => x.BirthDate == filter.BirthDate);
            }

            if (!string.IsNullOrEmpty(filter.Family))
            {
                patientQuery = patientQuery.Where(x => x.Name.Family != null && x.Name.Family.Contains(filter.Family));
            }

            if (!string.IsNullOrEmpty(filter.Given))
            {
                patientQuery = patientQuery.Where(x => x.Name.Given != null && x.Name.Given.Contains(filter.Given));
            }

            if (!string.IsNullOrEmpty(filter.City))
            {
                patientQuery = patientQuery.Where(patient => patient.Addresses.Any(a => !a.IsDeleted && a.City != null && a.City.Contains(filter.City)));
            }

            if (filter.CountryId.HasValue)
            {
                patientQuery = patientQuery.Where(patient => patient.Addresses.Any(a => !a.IsDeleted && a.CountryId == filter.CountryId));
            }

            if (!string.IsNullOrEmpty(filter.PostalCode))
            {
                patientQuery = patientQuery.Where(patient => patient.Addresses.Any(a => !a.IsDeleted && a.PostalCode != null && a.PostalCode.Contains(filter.PostalCode)));
            }

            patientQuery = FilterByIdentifier(patientQuery, filter.IdentifierType, filter.IdentifierValue);

            return patientQuery;
        }

        private IQueryable<Patient> FilterByIdentifier(IQueryable<Patient> patientEntities, string system, string value)
        {
            IQueryable<Patient> result = patientEntities;
            if (!string.IsNullOrEmpty(system) && !string.IsNullOrEmpty(value))
            {
                if (system.Equals(ResourceTypes.O4PatientId))
                {
                    if (Int32.TryParse(value, out int O4PatientId))
                    {
                        result = patientEntities.Where(x => x.PatientId.Equals(O4PatientId));
                    }
                }
                else
                {
                    result = patientEntities.Where(x => x.Identifiers.Any(y => !string.IsNullOrEmpty(y.System) && !string.IsNullOrEmpty(y.Value) && y.System.Equals(system) && y.Value.Equals(value)));
                }
            }

            return result;
        }

        public List<AutoCompletePatientData> GetPatientsFilteredByName(string searchValue)
        {
            Dictionary<int, Name> result = new Dictionary<int, Name>();

            return context.Patients
                .Where(x => x.Name.Given.ToLower().Contains(searchValue.ToLower())
                                        || x.Name.Family.ToLower().Contains(searchValue.ToLower()))
                .Select(x => new AutoCompletePatientData { PatientId = x.PatientId, Name = x.Name.Given, FamilyName = x.Name.Family })
                .ToList();
        }

        public List<AutoCompletePatientData> GetPatientsFilteredByFirstAndLastName(PatientSearchFilter patientSearchFilter)
        {
            var splitedSearchValue = patientSearchFilter.SearchValue.Split(Delimiters.delimiterCharacters);

            return context.Patients.ToList()
                .Select(x => new { Given = x.Name.Given.ToLower(), Family = x.Name.Family?.ToLower(), x.PatientId, x.BirthDate, GivenName = x.Name.Given, FamilyName = x.Name.Family })
                .Where(x => splitedSearchValue.Length > 1 ? x.Given.Contains(splitedSearchValue[0])
                                                            && x.Family != null ? x.Family.Contains(splitedSearchValue[1]) : (false
                                                            || x.Given.Contains(splitedSearchValue[1])
                                                            && x.Family != null) && x.Family.Contains(splitedSearchValue[0])
                                        : x.Family == null ? x.Given.Contains(patientSearchFilter.SearchValue) 
                                                             : x.Given.Contains(patientSearchFilter.SearchValue) || x.Family.Contains(patientSearchFilter.SearchValue))
                .Skip((patientSearchFilter.Page - 1) * patientSearchFilter.PageSize)
                .Take(patientSearchFilter.PageSize)
                .Select(x => new AutoCompletePatientData { PatientId = x.PatientId, Name = x.GivenName, FamilyName = x.FamilyName, BirthDate = x.BirthDate })
                .ToList();
        }

        private IQueryable<Patient> SortByField(IQueryable<Patient> result, PatientFilter filterData)
        {
            switch (filterData.ColumnName)
            {
                case AttributeNames.Gender:
                    string male = filterData.Genders[(int)Gender.Male];
                    string female = filterData.Genders[(int)Gender.Female];
                    string other = filterData.Genders[(int)Gender.Other];
                    string unknown = filterData.Genders[(int)Gender.Unknown];

                    if (filterData.IsAscending)
                        return result.OrderBy(x => (int)x.Gender == 0 ? male : (int)x.Gender == 1 ? female : (int)x.Gender == 2 ? other : unknown)
                                .Skip((filterData.Page - 1) * filterData.PageSize)
                                .Take(filterData.PageSize);
                    else
                        return result.OrderByDescending(x => (int)x.Gender == 0 ? male : (int)x.Gender == 1 ? female : (int)x.Gender == 2 ? other : unknown)
                                .Skip((filterData.Page - 1) * filterData.PageSize)
                                .Take(filterData.PageSize);
                case AttributeNames.Active:
                    string active = filterData.Activity[0];
                    string inactive = filterData.Activity[1];

                    if (filterData.IsAscending)
                        return result.OrderBy(x => x.Active ? active : inactive)
                                .Skip((filterData.Page - 1) * filterData.PageSize)
                                .Take(filterData.PageSize);
                    else
                        return result.OrderByDescending(x => x.Active == true ? active : inactive)
                                .Skip((filterData.Page - 1) * filterData.PageSize)
                                .Take(filterData.PageSize);
                default:
                    return SortTableHelper.OrderByField(result, filterData.ColumnName, filterData.IsAscending)
                            .Skip((filterData.Page - 1) * filterData.PageSize)
                            .Take(filterData.PageSize);
            }
        }
    }
}
