using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Api.DTOs.DfD.DataOut
{
    public class DfDCollectionDataOut<T>
    {
        public int Total { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public List<T> Data { get; set; }
    }
}
