﻿using sReportsV2.Domain.Entities.Encounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IEnumCommonService
    {
        List<EnumEntry> GetAll();
    }
}
