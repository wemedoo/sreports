using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.ThesaurusEntry.DataOut
{
    public class AdministrativeDataDataOut
    {
        public List<VersionDataOut> VersionHistory { get; set; }

        public AdministrativeDataDataOut()
        {
            VersionHistory = new List<VersionDataOut>();
        }

        public VersionDataOut GetInitialData()
        {
            return VersionHistory.OrderBy(x => x.CreatedOn).FirstOrDefault();
        }
    }
}