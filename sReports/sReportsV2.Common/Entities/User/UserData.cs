using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Common.Entities.User
{
    public class UserData
    {

        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? ActiveOrganization { get; set; }
        public List<int> Organizations { get; set; }

        public string GetName()
        {
            return FirstName + " " + LastName;
        }
    }
}
