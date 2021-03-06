using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Common
{
    public class AdministrativeData
    {        
        public List<Version> VersionHistory { get; set; }

        public AdministrativeData()
        {
            VersionHistory = new List<Version>();
        }

        public AdministrativeData(UserData userData, ThesaurusState? state)
        {
            userData = Ensure.IsNotNull(userData, nameof(userData));

            VersionHistory = new List<Version>();

            VersionHistory.Add(new Version()
            {
                CreatedOn = DateTime.Now,
                Type = VersionType.MAJOR,
                Id = Guid.NewGuid().ToString(),
                UserId = userData.Id,
                OrganizationId = userData.ActiveOrganization.GetValueOrDefault(),
                State = state
            });
        }

        public void UpdateVersionHistory(UserData userData, ThesaurusState? state)
        {
            userData = Ensure.IsNotNull(userData, nameof(userData));

            SetRevokedDateOfLastVersion();

            VersionHistory.Add(new Version()
            {
                CreatedOn = DateTime.Now,
                Type = VersionType.MAJOR,
                Id = Guid.NewGuid().ToString(),
                UserId = userData.Id,
                OrganizationId = userData.ActiveOrganization.GetValueOrDefault(),
                State = state
            });
        }

        private void SetRevokedDateOfLastVersion()
        {
            Version version = VersionHistory.LastOrDefault();
            if(version != null)
            {
                version.RevokedOn = DateTime.Now;
            }
        }

    }
}
