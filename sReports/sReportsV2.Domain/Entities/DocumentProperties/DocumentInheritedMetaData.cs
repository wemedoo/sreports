using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.DocumentProperties
{
    public class DocumentInheritedMetaData
    {
        public DocumentInheritedMetaDataPerson Designer { get; set; }
        public DocumentInheritedMetaDataPerson Reviewer { get; set; }
    }
}
