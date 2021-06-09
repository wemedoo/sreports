using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.OrganizationEntities
{
    public class OrganizationUsersCount
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public int UsersCount { get; set; }
        public List<OrganizationUsersCount> Children { get; set; }
        public int PartOf { get; set; }

        public string Country { get; set; }

        public bool FoundName(string term)
        {
            return this.OrganizationName.ToLower().Contains(term.ToLower()) || this.Children.Any(x => x.FoundName(term));
        }

        public bool FoundCountry(List<string> countries)
        {
            return countries.Contains(this.Country) || this.Children.Any(x => x.FoundCountry(countries));
        }
    }
}
