using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Patient.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.FormInstance.DataOut
{
    public class FormInstanceTableDataOut
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public sReportsV2.Domain.Entities.Form.Version Version { get; set; }
        public string Language { get; set; }
        public DateTime? EntryDatetime { get; set; }
        public DateTime LastUpdate { get; set; }
        public UserDataOut User { get; set; }
        public PatientTableDataOut Patient { get; set; }
        public int UserId { get; set; }

    }
}