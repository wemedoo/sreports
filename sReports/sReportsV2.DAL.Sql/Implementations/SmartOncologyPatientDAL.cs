using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.SmartOncologyPatient;
using sReportsV2.SqlDomain.Filter;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.SqlDomain.Implementations
{
    public class SmartOncologyPatientDAL : ISmartOncologyPatientDAL
    {
        private readonly SReportsContext context;
        public SmartOncologyPatientDAL(SReportsContext context)
        {
            this.context = context;
        }
        public void Delete(int patientId)
        {
            SmartOncologyPatient fromDb = GetById(patientId);
            if (fromDb != null)
            {
                fromDb.IsDeleted = true;
                fromDb.SetLastUpdate();
                context.SaveChanges();
            }
        }

        public List<SmartOncologyPatient> GetAll(SmartOncologyPatientFilter filter)
        {
            var result = this.GetPatientFiltered(filter)
                .OrderByDescending(x => x.SmartOncologyPatientId)
                //.Skip((filter.Page - 1) * filter.PageSize)
                //.Take(filter.PageSize)
                .ToList();

            return result;
        }

        public int GetAllEntriesCount(SmartOncologyPatientFilter filter)
        {
            return this.GetPatientFiltered(filter).Count();
        }

        public SmartOncologyPatient GetById(int id)
        {
            return context.SmartOncologyPatients
                .FirstOrDefault(x => x.SmartOncologyPatientId == id);
        }

        public List<SmartOncologyPatient> GetPatientsByName(string name)
        {
            return GetPatientFiltered(new SmartOncologyPatientFilter() { Name = name}).ToList();
        }

        public void InsertOrUpdate(SmartOncologyPatient patient)
        {
            if (patient.SmartOncologyPatientId == 0)
            {
                context.SmartOncologyPatients.Add(patient);
            }
            else
            {
                patient.SetLastUpdate();
            }

            context.SaveChanges();
        }

        private IQueryable<SmartOncologyPatient> GetPatientFiltered(SmartOncologyPatientFilter filter)
        {
            IQueryable<SmartOncologyPatient> patientQuery = context.SmartOncologyPatients
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(filter.Name))
            {
                patientQuery = patientQuery.Where(x => (x.Name.Family != null && x.Name.Family.Contains(filter.Name)) || (x.Name.Given != null && x.Name.Given.Contains(filter.Name)));
            }

            if (filter.BirthDate != null)
            {
                patientQuery = patientQuery.Where(x => x.BirthDate == filter.BirthDate);
            }

            return patientQuery;
        }
    }
}
