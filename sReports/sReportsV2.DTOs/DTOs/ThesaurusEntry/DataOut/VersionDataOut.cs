using sReportsV2.Domain.Entities.Common;
using sReportsV2.Common.Enums;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.User.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.ThesaurusEntry.DataOut
{
    public class VersionDataOut
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public UserShortInfoDataOut User {get;set;}
        public VersionType Type { get; set; }
        public OrganizationDataOut Organization { get; set; }
        public ThesaurusState? State { get; set; }

        public string GetStateColor(ThesaurusState? status)
        {
            string color;

            if (status == ThesaurusState.Production)
                color = "production-state";
            else if (status == ThesaurusState.Deprecated)
                color = "depracated-state";
            else if (status == ThesaurusState.Disabled)
                color = "disabled-state";
            else
                color = "administrative-state";

            return color;
        }

    }
}