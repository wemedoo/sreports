using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.ThesaurusEntry
{
    public class AdministrativeData
    {
        public int Id { get; set; }
        public List<Version> VersionHistory { get; set; }
        public int ThesaurusEntryId { get; set; }
        public AdministrativeData() { }
        public AdministrativeData(UserData userData, ThesaurusState? state)
        {
            VersionHistory = new List<Version>();

            VersionHistory.Add(new Version()
            {
                CreatedOn = DateTime.Now,
                Type = VersionType.MAJOR,
                UserId = userData.Id,
                OrganizationId = userData.ActiveOrganization.GetValueOrDefault(),
                State = state
            });
        }

        public void UpdateVersionHistory(UserData userData, ThesaurusState? state)
        {
            VersionHistory = VersionHistory != null ? VersionHistory : new List<Version>();

            SetRevokedDateOfLastVersion();

            VersionHistory.Add(new Version()
            {
                CreatedOn = DateTime.Now,
                Type = VersionType.MAJOR,
                UserId = userData.Id,
                OrganizationId = userData.ActiveOrganization.GetValueOrDefault(),
                State = state
            });
        }

        private void SetRevokedDateOfLastVersion()
        {
            Version version = VersionHistory.LastOrDefault();
            if (version != null)
            {
                version.RevokedOn = DateTime.Now;
            }
        }
    }
}
