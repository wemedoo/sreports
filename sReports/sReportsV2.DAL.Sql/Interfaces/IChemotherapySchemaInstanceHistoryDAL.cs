using sReportsV2.Domain.Sql.Entities.ChemotherapySchemaInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IChemotherapySchemaInstanceHistoryDAL
    {
        void InsertOrUpdate(ChemotherapySchemaInstanceVersion chemotherapySchemaInstanceHistory);
        ChemotherapySchemaInstanceVersion GetById(int id);
        List<ChemotherapySchemaInstanceVersion> ViewHistoryOfDayDose(int chemotherapySchemaId, int dayNumber);
    }
}
