using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Common
{
    public class AddressDTO
    {
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Street { get; set; }
        public AddressDTO() { }

        public AddressDTO(string city, string state, string postalCode, string country)
        {
            this.City = city;
            this.State = state;
            this.PostalCode = postalCode;
            this.Country = country;
        }
    }
}