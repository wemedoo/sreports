using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataOut
{
    public class ChemotherapySchemaInstanceHistoryDataOut
    {
        public int FirstDelayDay { get; set; }
        public DateTime StartDayDate { get; set; }
        public List<ChemotherapySchemaInstanceVersionDataOut> History { get; set; } = new List<ChemotherapySchemaInstanceVersionDataOut>();

        public void CalculateDelayedDateByVersions()
        {
            DateTime currentDate = StartDayDate;
            foreach (var chemotherapySchemaInstanceVersion in History)
            {
                currentDate = currentDate.AddDays(chemotherapySchemaInstanceVersion.DelayFor);
                chemotherapySchemaInstanceVersion.DelayedDate = currentDate;
            }
        }
    }
}
