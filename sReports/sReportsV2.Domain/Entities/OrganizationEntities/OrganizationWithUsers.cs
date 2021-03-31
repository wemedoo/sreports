using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sReportsV2.Domain.Entities.UserEntities;
namespace sReportsV2.Domain.Entities.OrganizationEntities
{
    public class OrganizationWithUsers : Organization
    {
        public List<UserEntities.User> Users { get; set; }
    }
}
