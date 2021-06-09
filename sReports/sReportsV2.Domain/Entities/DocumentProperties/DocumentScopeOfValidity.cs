using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.DocumentProperties
{
    public class DocumentScopeOfValidity
    {
        public DocumentScopeOfValidityEnum? ScopeOfValidity { get; set; }
        public string Value { get; set; }
    }
}
