using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace sReportsV2.Domain.Sql.Entities.Patient
{
    public class Patient : PatientBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("PatientId")]
        public int PatientId { get; set; }
        public int OrganizationId { get; set; }
        public List<PatientAddress> Addresses { get; set; }
        public List<PatientTelecom> PatientTelecoms { get; set; }
        public int? CitizenshipId { get; set; }
        [ForeignKey("CitizenshipId")]
        public CustomEnum Citizenship { get; set; }
        public int? ReligionId { get; set; }
        [ForeignKey("ReligionId")]
        public CustomEnum Religion { get; set; }
        public DateTime? DeceasedDateTime { get; set; }
        public bool? Deceased { get; set; }
        

        public void SetGenderFromString(string gender)
        {
            switch (gender)
            {
                case "Male":
                    this.Gender = Gender.Male; break;
                case "Female":
                    this.Gender = Gender.Female; break;
                case "Other":
                    this.Gender = Gender.Other; break;
                case "Unknown":
                    this.Gender = Gender.Unknown; break;
                default:
                    this.Gender = Gender.Unknown; break;
            }
        }

        public void Copy(Patient patient)
        {
            base.Copy(patient);
            
            OrganizationId = patient.OrganizationId;
            CitizenshipId = patient.CitizenshipId;
            ReligionId = patient.ReligionId;
            Deceased = patient.Deceased;
            DeceasedDateTime = patient.DeceasedDateTime;

            CopyAddresses(patient.Addresses);
            CopyTelecoms(patient.PatientTelecoms);
        }

        private void CopyAddresses(List<PatientAddress> upcomingPatientAddresses)
        {
            if (upcomingPatientAddresses != null)
            {
                DeleteExistingRemovedAddresses(upcomingPatientAddresses);
                AddNewOrUpdateOldAddresses(upcomingPatientAddresses);
            }
        }

        private void DeleteExistingRemovedAddresses(List<PatientAddress> upcomingPatientAddresses)
        {
            foreach (var address in Addresses)
            {
                var remainingAddress = upcomingPatientAddresses.Any(x => x.PatientAddressId == address.PatientAddressId);
                if (!remainingAddress)
                {
                    address.IsDeleted = true;
                }
            }
        }

        private void AddNewOrUpdateOldAddresses(List<PatientAddress> upcomingPatientAddresses)
        {
            foreach (var patientAddress in upcomingPatientAddresses)
            {
                if (patientAddress.PatientAddressId == 0)
                {
                    Addresses.Add(patientAddress);
                }
                else
                {
                    var dbPatientAddress = Addresses.FirstOrDefault(x => x.PatientAddressId == patientAddress.PatientAddressId && !x.IsDeleted);
                    if (dbPatientAddress != null)
                    {
                        dbPatientAddress.Copy(patientAddress);
                    }
                }
            }
        }

        private void CopyTelecoms(List<PatientTelecom> upcomingPatientTelecoms)
        {
            if (upcomingPatientTelecoms != null)
            {
                DeleteExistingRemovedTelecoms(upcomingPatientTelecoms);
                AddNewOrUpdateOldTelecoms(upcomingPatientTelecoms);
            }
        }

        private void DeleteExistingRemovedTelecoms(List<PatientTelecom> upcomingPatientTelecoms)
        {
            foreach (var telecom in PatientTelecoms)
            {
                var remainingTelecom = upcomingPatientTelecoms.Any(x => x.PatientTelecomId == telecom.PatientTelecomId);
                if (!remainingTelecom)
                {
                    telecom.IsDeleted = true;
                }
            }
        }

        private void AddNewOrUpdateOldTelecoms(List<PatientTelecom> upcomingPatientTelecoms)
        {
            foreach (var patientTelecom in upcomingPatientTelecoms)
            {
                if (patientTelecom.PatientTelecomId == 0)
                {
                    PatientTelecoms.Add(patientTelecom);
                }
                else
                {
                    var dbPatientTelecom = PatientTelecoms.FirstOrDefault(x => x.PatientTelecomId == patientTelecom.PatientTelecomId && !x.IsDeleted);
                    if (dbPatientTelecom != null)
                    {
                        dbPatientTelecom.Copy(patientTelecom);
                    }
                }
            }
        }
    }
}
