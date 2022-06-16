using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IPatientBLL
    {
        ResourceCreatedDTO Insert(PatientDataIn patientDataIn);
    }
}
