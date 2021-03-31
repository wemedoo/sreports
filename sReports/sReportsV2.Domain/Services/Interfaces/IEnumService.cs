using sReportsV2.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IEnumService
    {
        Dictionary<string, List<EnumData>> GetDocumentPropertiesEnums();
    }
}
