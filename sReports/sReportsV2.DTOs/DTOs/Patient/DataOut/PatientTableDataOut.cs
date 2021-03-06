using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Patient.DataOut
{
    public class PatientTableDataOut
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }

        public PatientTableDataOut() { }
        public PatientTableDataOut(string firstName, string lastName, DateTime birthDate)
        {

            this.FirstName = firstName;
            this.LastName = lastName;
            this.BirthDate = birthDate;
        }
    }
}