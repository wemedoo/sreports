using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Implementations
{
    public class MedicationDoseTypeDAL : IMedicationDoseTypeDAL
    {
        private readonly SReportsContext context;
        public MedicationDoseTypeDAL(SReportsContext context)
        {
            this.context = context;
        }
        public List<MedicationDoseType> GetAll()
        {
            return context.MedicationDoseTypes.ToList();
        }

        public int GetAllCount()
        {
            return context.MedicationDoseTypes.Count();
        }

        public MedicationDoseType GetById(int id)
        {
            return context.MedicationDoseTypes.FirstOrDefault(x => x.MedicationDoseTypeId == id);
        }

        public MedicationDoseType GetByName(string name)
        {
            return context.MedicationDoseTypes.FirstOrDefault(x => x.Type.Equals(name));
        }

        public void InsertMany(List<MedicationDoseType> bodySurfaceCalculationFormulas)
        {
            DataTable bodySurfaceCalculationFormulaRowTable = new DataTable();
            bodySurfaceCalculationFormulaRowTable.Columns.Add(new DataColumn("Type", typeof(string)));
            bodySurfaceCalculationFormulaRowTable.Columns.Add(new DataColumn("Intervals", typeof(string)));

            foreach (var bodySurfaceCalculationFormula in bodySurfaceCalculationFormulas)
            {
                DataRow bodySurfaceCalculationFormulaRow = bodySurfaceCalculationFormulaRowTable.NewRow();
                bodySurfaceCalculationFormulaRow["Type"] = bodySurfaceCalculationFormula.Type;
                bodySurfaceCalculationFormulaRow["Intervals"] = bodySurfaceCalculationFormula.Intervals;
                bodySurfaceCalculationFormulaRowTable.Rows.Add(bodySurfaceCalculationFormulaRow);
            }


            string connection = ConfigurationManager.AppSettings["Sql"];
            SqlConnection con = new SqlConnection(connection);

            SqlBulkCopy objbulk = new SqlBulkCopy(con)
            {
                BulkCopyTimeout = 0,
                DestinationTableName = "MedicationDoseTypes"
            };
            objbulk.ColumnMappings.Add("Type", "Type");
            objbulk.ColumnMappings.Add("Intervals", "Intervals");

            con.Open();
            objbulk.WriteToServer(bodySurfaceCalculationFormulaRowTable);
            con.Close();
        }

        public void InsertOrUpdate(MedicationDoseType medicationDoseType)
        {
            if(medicationDoseType.MedicationDoseTypeId == 0)
            {
                context.MedicationDoseTypes.Add(medicationDoseType);
            }
            context.SaveChanges();
        }
    }
}
