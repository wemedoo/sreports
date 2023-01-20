﻿using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IBodySurfaceCalculationFormulaDAL
    {
        List<BodySurfaceCalculationFormula> GetAll();
        int GetAllCount();
        void InsertMany(List<BodySurfaceCalculationFormula> bodySurfaceCalculationFormulas);
    }
}