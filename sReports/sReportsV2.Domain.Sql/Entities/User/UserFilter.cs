using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.User
{
    public class UserFilter
    {
        public bool ShowUnassignedUsers { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string ColumnName { get; set; }
        public bool IsAscending { get; set; }
    }
}
