using System;

namespace sReportsV2.DTOs.DTOs.Form.DataOut
{
    public abstract class FormStatusAbstractDataOut
    {
        public abstract dynamic StatusValue { get; }
        public abstract DateTime CreatedDateTime { get; }
        public abstract string CreatedName { get;  }
    }
}
