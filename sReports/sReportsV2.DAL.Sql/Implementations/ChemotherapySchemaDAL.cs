using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using sReportsV2.SqlDomain.Filter;
using sReportsV2.Common.Helpers;

namespace sReportsV2.SqlDomain.Implementations
{
    public class ChemotherapySchemaDAL : IChemotherapySchemaDAL
    {
        private readonly SReportsContext context;

        public ChemotherapySchemaDAL(SReportsContext context)
        {
            this.context = context;
        }

        public void Delete(int id)
        {
            ChemotherapySchema fromDb = context.ChemotherapySchemas.FirstOrDefault(x => x.ChemotherapySchemaId == id);
            if (fromDb != null)
            {
                fromDb.IsDeleted = true;
                fromDb.SetLastUpdate();
                context.SaveChanges();
            }
        }

        public IQueryable<ChemotherapySchema> FilterByName(string name)
        {
            return context.ChemotherapySchemas.Where(x => !x.IsDeleted && x.Name.ToLower().Contains(name.ToLower()));
        }

        public List<ChemotherapySchema> GetAll(ChemotherapySchemaFilter chemotherapySchemaFilter)
        {
            IQueryable<ChemotherapySchema> result = GetChemotherapySchemasFiltered(chemotherapySchemaFilter);

            if (chemotherapySchemaFilter.ColumnName != null)
            {
                result = SortTableHelper.OrderByField(result, chemotherapySchemaFilter.ColumnName, chemotherapySchemaFilter.IsAscending)
                    .Skip((chemotherapySchemaFilter.Page - 1) * chemotherapySchemaFilter.PageSize)
                    .Take(chemotherapySchemaFilter.PageSize);
            }
            else
            {
                result = result.OrderByDescending(x => x.EntryDatetime)
                   .Skip((chemotherapySchemaFilter.Page - 1) * chemotherapySchemaFilter.PageSize)
                   .Take(chemotherapySchemaFilter.PageSize);
            }

            return result.ToList();
        }

        public long GetAllFilteredCount(ChemotherapySchemaFilter chemotherapySchemaFilter)
        {
            return GetChemotherapySchemasFiltered(chemotherapySchemaFilter).Count();
        }

        public int GetAllCount()
        {
            return context.ChemotherapySchemas.Count();
        }

        public ChemotherapySchema GetById(int id)
        {
            return context.ChemotherapySchemas
                .Include(x => x.Creator)
                .Include(x => x.Indications)
                .Include(x => x.LiteratureReferences)
                .Include(x => x.Medications)
                .Include("Medications.MedicationDoses")
                .FirstOrDefault(x => x.ChemotherapySchemaId == id);
        }

        public ChemotherapySchema GetSchemaDefinition(int id)
        {
            return context.ChemotherapySchemas
                .Include("Medications")
                .Include("Medications.Unit")
                .Include("Medications.MedicationDoses")
                .Include("Medications.MedicationDoses.Unit")
                .Include("Medications.MedicationDoses.MedicationDoseTimes")
                .FirstOrDefault(x => x.ChemotherapySchemaId == id);
        }

        public void InsertOrUpdate(ChemotherapySchema chemotherapySchema)
        {
            if (chemotherapySchema.ChemotherapySchemaId == 0)
            {
                context.ChemotherapySchemas.Add(chemotherapySchema);
            }
            else
            {
                context.Entry(chemotherapySchema).OriginalValues["RowVersion"] = chemotherapySchema.RowVersion;
                chemotherapySchema.SetLastUpdate();
            }

            context.SaveChanges();
        }

        public void InsertMany(IEnumerable<ChemotherapySchema> chemotherapySchemas)
        {
            context.ChemotherapySchemas.AddRange(chemotherapySchemas);
            context.SaveChanges();
        }

        public byte[] GetRowVersion(int id)
        {
            return context.ChemotherapySchemas
                .FirstOrDefault(x => x.ChemotherapySchemaId == id).RowVersion;
        }

        private IQueryable<ChemotherapySchema> GetChemotherapySchemasFiltered(ChemotherapySchemaFilter chemotherapySchemaFilter)
        {
            IQueryable<ChemotherapySchema> query = this.context.ChemotherapySchemas
                .Include(sh => sh.Indications)
                .Where(sh => !sh.IsDeleted 
                && (chemotherapySchemaFilter.Name == null || sh.Name.ToLower().Contains(chemotherapySchemaFilter.Name.ToLower()))
                );

            query = FilterByIndication(query, chemotherapySchemaFilter.Indication);

            return query;
        }

        private IQueryable<ChemotherapySchema> FilterByIndication(IQueryable<ChemotherapySchema> query, string indication)
        {
            IQueryable<ChemotherapySchema> result = query;
            if (!string.IsNullOrEmpty(indication))
            {
                  result = query
                        .Where(ch => ch.Indications.Any(ind => !ind.IsDeleted && ind.Name.ToLower().Contains(indication.ToLower())));
            }

            return result;
        }
    }
}
