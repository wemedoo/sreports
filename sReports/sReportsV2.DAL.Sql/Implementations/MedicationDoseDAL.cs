using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace sReportsV2.SqlDomain.Implementations
{
    public class MedicationDoseDAL : IMedicationDoseDAL
    {
        private readonly SReportsContext context;

        public MedicationDoseDAL(SReportsContext context)
        {
            this.context = context;
        }

        public MedicationDose GetById(int id)
        {
            return context.MedicationDoses
                .Include(m => m.MedicationDoseTimes)
                .FirstOrDefault(m => m.MedicationDoseId == id);
        }

        public void Delete(int id)
        {
            MedicationDose fromDb = context.MedicationDoses.FirstOrDefault(x => x.MedicationDoseId == id);
            if (fromDb != null)
            {
                fromDb.IsDeleted = true;
                context.SaveChanges();
            }
        }

        public void InsertOrUpdate(MedicationDose medicationDose)
        {
            if (medicationDose.MedicationDoseId == 0)
            {
                context.MedicationDoses.Add(medicationDose);
            }
            context.SaveChanges();
        }
    }
}
