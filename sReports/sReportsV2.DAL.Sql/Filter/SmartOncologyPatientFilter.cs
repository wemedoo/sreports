using System;

namespace sReportsV2.SqlDomain.Filter
{
    public class SmartOncologyPatientFilter
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Name { get; set; }
    }
}
