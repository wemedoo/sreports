using System.ComponentModel.DataAnnotations;

namespace sReportsV2.DTOs.Common
{
    public class AddressDataIn
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string City { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        [StringLength(10)]
        public string PostalCode { get; set; }

        //[StringLength(50)]
        //public string Country { get; set; }

        [StringLength(200)]
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public int? AddressTypeId { get; set; }
        public int? CountryId { get; set; }
    }
}
