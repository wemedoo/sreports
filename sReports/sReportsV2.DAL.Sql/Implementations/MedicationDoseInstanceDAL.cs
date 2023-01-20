using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchemaInstance;
using sReportsV2.SqlDomain.Interfaces;
using System.Linq;
using System.Data.Entity;

namespace sReportsV2.SqlDomain.Implementations
{
    public class MedicationDoseInstanceDAL : IMedicationDoseInstanceDAL
    {
        private readonly SReportsContext context;

        public MedicationDoseInstanceDAL(SReportsContext context)
        {
            this.context = context;
        }

        public MedicationDoseInstance GetById(int id)
        {
            return context.MedicationDoseInstances
                                .Include(m => m.MedicationDoseTimes)
                .FirstOrDefault(x => x.MedicationDoseInstanceId == id);
        }

        public void InsertOrUpdate(MedicationDoseInstance medicationDoseInstance)
        {
            if(medicationDoseInstance.MedicationDoseInstanceId == 0)
            {
                context.MedicationDoseInstances.Add(medicationDoseInstance);
            }
            context.SaveChanges();
        }

        public int Delete(int id)
        {
            MedicationDoseInstance fromDb = context.MedicationDoseInstances.FirstOrDefault(x => x.MedicationDoseInstanceId == id);
            if (fromDb != null)
            {
                fromDb.IsDeleted = true;
                context.SaveChanges();
                return id;
            }
            else
            {
                return 0;
            }
        }
    }
}
