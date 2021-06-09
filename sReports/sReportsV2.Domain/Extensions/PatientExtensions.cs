using Hl7.Fhir.Model;
using sReportsV2.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Extensions
{
    public static class PatientExtensions
    {
        public static void SetGender(this Patient patient, string gender)
        {
            patient = Ensure.IsNotNull(patient, nameof(patient));

            switch (gender)
            {
                case "Male":
                    patient.Gender = AdministrativeGender.Male;
                    break;
                case "Female":
                    patient.Gender = AdministrativeGender.Female;
                    break;
                case "Other":
                    patient.Gender = AdministrativeGender.Other;
                    break;
                default:
                    patient.Gender = AdministrativeGender.Unknown;
                    break;
            }
        }
    }
}
