using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Umls.DatOut
{
    public class UmlsDataOut<T>
    {
        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public int PageCount { get; set; }

        public T Result { get; set; }
    }
}