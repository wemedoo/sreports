using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.DocumentProperties
{
    public class DocumentClass
    {
        public DocumentClassEnum? Class { get; set; }
        public string Other { get; set; }
    }
}
