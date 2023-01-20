using sReportsV2.DTOs.Form.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.FormDistribution.DataOut
{
    public class FormDistributionParameterizationDataOut
    {
        public FormDataOut Form{ get; set; }
        public FormDistributionDataOut FormDistribution { get; set; }
    }
}
