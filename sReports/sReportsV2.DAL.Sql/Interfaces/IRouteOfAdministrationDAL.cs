using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IRouteOfAdministrationDAL
    {
        int GetAllCount();        
        void InsertMany(List<RouteOfAdministration> bodySurfaceCalculationFormulas);
        IQueryable<RouteOfAdministration> FilterByName(string name);
        RouteOfAdministration GetById(int id);
    }
}
