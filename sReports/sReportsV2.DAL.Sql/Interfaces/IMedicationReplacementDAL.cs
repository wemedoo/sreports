using sReportsV2.Domain.Sql.Entities.ChemotherapySchemaInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IMedicationReplacementDAL
    {
        void InsertInBatch(List<MedicationReplacement> medicationReplacements);
        IQueryable<MedicationReplacement> GetByMedication(int medicationInstanceId);
        bool DoesMedicationParticipateInAnyReplacment(int medicationInstanceId);
    }
}
