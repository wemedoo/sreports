using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapters.Resources
{
    public static class PdfParserType
    {
        #region FieldFhirTypes

        public const string DocumentDate = "DocumentDate";

        //basic patient info
        public const string Family = "Family";
        public const string Unknown = "Unknown";
        public const string Name = "Name";
        public const string BirthDate = "BirthDate";
        public const string Language = "Language";
        public const string Gender = "Gender";
        public const string Relationship = "Relationship";

        //contact info
        public const string ContactFamily = "ContactFamily";
        public const string ContactName = "ContactName";
        public const string ContactGender = "ContactGender";

        //identifier 
        public const string IdentifierName = "IdentifierName";
        public const string IdentifierValue = "IdentifierValue";
        public const string IdentifierUse = "IdentifierUse";

        //address
        public const string City = "City";
        public const string State = "State";
        public const string PostalCode = "PostalCode";
        public const string Country = "Country";
        public const string Street = "Street";

        //contact address
        public const string ContactCity = "ContactCity";
        public const string ContactState = "ContactState";
        public const string ContactPostalCode = "ContactPostalCode";
        public const string ContactCountry = "ContactCountry";
        public const string ContactStreet = "ContactStreet";

        //contact telecom
        public const string ContactPhone = "ContactPhone";
        public const string ContactEmail = "ContactEmail";
        public const string ContactUrl = "ContactUrl";
        public const string ContactFax = "ContactFax";
        public const string ContactSms = "ContactSms"; 
        public const string ContactOther = "ContactOther";
        public const string ContactPager = "ContactPager";

        public const string ContactPhoneUse = "ContactPhoneUse";
        public const string ContactEmailUse = "ContactEmailUse";
        public const string ContactUrlUse = "ContactUrlUse";
        public const string ContactFaxUse = "ContactFaxUse";
        public const string ContactSmsUse = "ContactSmsUse";
        public const string ContactOtherUse = "ContactOtherUse";
        public const string ContactPagerUse = "ContactPagerUse";

        //patient telecom
        public const string Phone = "Phone";
        public const string Email = "Email";
        public const string Url = "Url";
        public const string Fax = "Fax";
        public const string Sms = "Sms";
        public const string Other = "Other";
        public const string Pager = "Pager";

        public const string PhoneUse = "PhoneUse";
        public const string EmailUse = "EmailUse";
        public const string UrlUse = "UrlUse";
        public const string FaxUse = "FaxUse";
        public const string SmsUse = "SmsUse";
        public const string OtherUse = "OtherUse";
        public const string PagerUse = "PagerUse";

        #endregion
    }
}
