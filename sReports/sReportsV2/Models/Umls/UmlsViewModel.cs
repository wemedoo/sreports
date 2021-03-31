using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.Models.Umls
{
    public class UmlsViewModel<T>
    {
        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public int PageCount { get; set; }

        public T Result { get; set; }
    }
}