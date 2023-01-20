using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Common
{
    public class AddressDTO
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public int? AddressTypeId { get; set; }
        public int? CountryId { get; set; }
        public string GetAddressFormated() 
        {
            return $"{this.Street}, {this.StreetNumber}";
        }
        public string GetAddressPreview()
        {
            return $"{City}, {PostalCode}, {Country}";
        }
    }
}