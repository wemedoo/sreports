using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Pagination
{
    public class PaginationDataOut<T, TIn>
    {
        public int Count { get; set; }
        public TIn DataIn { get; set; }
        public List<T> Data { get; set; }
    }
}