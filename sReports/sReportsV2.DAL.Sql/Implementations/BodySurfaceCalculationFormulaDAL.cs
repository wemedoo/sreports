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
    public class BodySurfaceCalculationFormulaDAL : IBodySurfaceCalculationFormulaDAL
    {
        private readonly SReportsContext context;
        public BodySurfaceCalculationFormulaDAL(SReportsContext context)
        {
            this.context = context;
        }
        public List<BodySurfaceCalculationFormula> GetAll()
        {
            return context.BodySurfaceCalculationFormulas.ToList();
        }

        public int GetAllCount()
        {
            return context.BodySurfaceCalculationFormulas.Count();

        }

        public void InsertMany(List<BodySurfaceCalculationFormula> bodySurfaceCalculationFormulas)
        {
            DataTable bodySurfaceCalculationFormulaRowTable = new DataTable();
            bodySurfaceCalculationFormulaRowTable.Columns.Add(new DataColumn("Name", typeof(string)));
            bodySurfaceCalculationFormulaRowTable.Columns.Add(new DataColumn("Formula", typeof(string)));

            foreach (var bodySurfaceCalculationFormula in bodySurfaceCalculationFormulas)
            {
                DataRow bodySurfaceCalculationFormulaRow = bodySurfaceCalculationFormulaRowTable.NewRow();
                bodySurfaceCalculationFormulaRow["Name"] = bodySurfaceCalculationFormula.Name;
                bodySurfaceCalculationFormulaRow["Formula"] = bodySurfaceCalculationFormula.Formula;
                bodySurfaceCalculationFormulaRowTable.Rows.Add(bodySurfaceCalculationFormulaRow);
            }


            string connection = ConfigurationManager.AppSettings["Sql"];
            SqlConnection con = new SqlConnection(connection);

            SqlBulkCopy objbulk = new SqlBulkCopy(con)
            {
                BulkCopyTimeout = 0,
                DestinationTableName = "BodySurfaceCalculationFormulas"
            };
            objbulk.ColumnMappings.Add("Name", "Name");
            objbulk.ColumnMappings.Add("Formula", "Formula");

            con.Open();
            objbulk.WriteToServer(bodySurfaceCalculationFormulaRowTable);
            con.Close();
        }
    }
}
