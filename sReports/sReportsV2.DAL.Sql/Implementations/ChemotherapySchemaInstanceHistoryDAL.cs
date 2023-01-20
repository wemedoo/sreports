using sReportsV2.Common.SmartOncologyEnums;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchemaInstance;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Implementations
{
    public class ChemotherapySchemaInstanceHistoryDAL : IChemotherapySchemaInstanceHistoryDAL
    {
        private readonly SReportsContext context;

        public ChemotherapySchemaInstanceHistoryDAL(SReportsContext context)
        {
            this.context = context;
        }

        public ChemotherapySchemaInstanceVersion GetById(int id)
        {
            return context.ChemotherapySchemaInstanceVersions
                .FirstOrDefault(x => x.ChemotherapySchemaInstanceVersionId == id);
        }

        public List<ChemotherapySchemaInstanceVersion> ViewHistoryOfDayDose(int chemotherapySchemaId, int dayNumber)
        {
            return context.ChemotherapySchemaInstanceVersions
                .Where(x => !x.IsDeleted 
                && x.ChemotherapySchemaInstanceId == chemotherapySchemaId && dayNumber >= x.FirstDelayDay && x.ActionType == ChemotherapySchemaInstanceActionType.DelayDose)
                .ToList();
        }

        public void InsertOrUpdate(ChemotherapySchemaInstanceVersion chemotherapySchemaInstanceHistory)
        {
            if (chemotherapySchemaInstanceHistory.ChemotherapySchemaInstanceVersionId == 0)
            {
                context.ChemotherapySchemaInstanceVersions.Add(chemotherapySchemaInstanceHistory);
            }
            else
            {
                chemotherapySchemaInstanceHistory.SetLastUpdate();
            }
            context.SaveChanges();
        }
    }
}
