using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.Common
{
    public class AutoCompletePatientData 
    {
        public int PatientId { get; set; }
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public DateTime? BirthDate { get; set; }
    }

    public class AutoCompleteUserData
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
    }
}
