using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.DocumentProperties
{
    public class DocumentInheritedMetaDataPerson
    {
        public DocumentInheritedMetaDataInstitution Institution { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
    }
}
