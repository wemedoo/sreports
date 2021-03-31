using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.PatientEntities
{
    public class Address
    {
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public string Street { get; set; }
        public Address() { }

        public Address(string city,string state, string postalCode, string country)
        {
            this.City = city;
            this.State = state;
            this.PostalCode = postalCode;
            this.Country = country;
        }

        public override string ToString()
        {
            return Country + "-" + PostalCode + " " + City;
        }
    }
}
