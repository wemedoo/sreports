using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchemaInstance;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;

namespace sReportsV2.SqlDomain.Implementations
{
    public class MedicationInstanceDAL : IMedicationInstanceDAL
    {
        private readonly SReportsContext context;

        public MedicationInstanceDAL(SReportsContext context)
        {
            this.context = context;
        }
        public MedicationInstance GetById(int id)
        {
            return context.MedicationInstances
                .Include("MedicationDoses")
                .Include("MedicationDoses.MedicationDoseTimes")
                .FirstOrDefault(x => x.MedicationInstanceId == id);
        }

        public List<string> GetNameByIds(List<int> ids)
        {
            return context.MedicationInstances
                .Include(x => x.Medication)
                .Where(x => ids.Contains(x.MedicationInstanceId))
                .Select(x => x.Medication.Name).ToList();
        }

        public int Delete(int id)
        {
            MedicationInstance fromDb = context.MedicationInstances.FirstOrDefault(x => x.MedicationInstanceId == id);
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

        public void InsertOrUpdate(MedicationInstance medicationInstance)
        {
            if (medicationInstance.MedicationInstanceId == 0)
            {
                context.MedicationInstances.Add(medicationInstance);
            }
            else
            {
                context.Entry(medicationInstance).OriginalValues["RowVersion"] = medicationInstance.RowVersion;
                medicationInstance.SetLastUpdate();
            }
            context.SaveChanges();
        }

        public byte[] GetRowVersion(int id)
        {
            return context.MedicationInstances
                .FirstOrDefault(x => x.MedicationInstanceId == id).RowVersion;
        }

        public IQueryable<MedicationInstance> FilterByNameAndChemotherapySchemaInstanceAndType(string name, int chemotherapySchemaInstanceId, bool isSupportiveMedication)
        {
            return context.MedicationInstances
                .Include(x => x.Medication)
                .Where(x => !x.IsDeleted
                && x.Medication.Name.ToLower().Contains(name.ToLower())
                && x.ChemotherapySchemaInstanceId == chemotherapySchemaInstanceId
                && x.Medication.IsSupportiveMedication.Equals(isSupportiveMedication)
                )
                .GroupJoin(context.MedicationReplacements, medicationInstance => medicationInstance.ChemotherapySchemaInstanceId, medicationReplacement => medicationReplacement.ChemotherapySchemaInstanceId, (medicationInstance, medicationReplacement) => new
                {
                    MedicationInstance = medicationInstance,
                    MedicationReplacement = medicationReplacement
                })
                .Where(x => !x.MedicationReplacement.Any(y => y.ReplaceMedicationId == x.MedicationInstance.MedicationInstanceId))
                .Select(x => x.MedicationInstance)
                ;
        }
    }
}
