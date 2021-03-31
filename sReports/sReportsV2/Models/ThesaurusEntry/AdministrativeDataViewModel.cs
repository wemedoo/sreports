using sReportsV2.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.Models.ThesaurusEntry
{
    public class AdministrativeDataViewModel
    {
        public List<VersionViewModel> VersionHistory { get; set; }

        public AdministrativeDataViewModel()
        {
            VersionHistory = new List<VersionViewModel>();
        }

        public VersionViewModel GetInitialData()
        {
            return VersionHistory.OrderBy(x => x.CreatedOn).FirstOrDefault();
        }
    }
}