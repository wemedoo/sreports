using sReportsV2.Domain.Entities.Common;
using sReportsV2.DTOs.CustomEnum;
using sReportsV2.DTOs.Encounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface ICustomEnumBLL
    {
        Dictionary<string, List<EnumData>> GetDocumentPropertiesEnums();
        void Insert(CustomEnumDataIn enumDataIn, int activeOrganization);
        void Delete(CustomEnumDataIn customEnumDataIn);
    }
}
